#r "nuget: FsHttp, 15.0.1"

open FsHttp

Fsi.disableDebugLogs ()

let healthReq () =
    http { GET "http://localhost:8085/health" } |> Request.sendAsync |> Async.Ignore

let pingReq () =
    http { GET "http://localhost:8085/ping/foo" }
    |> Request.sendAsync
    |> Async.Ignore

let tradeReq () =
    http {
        POST "http://localhost:8085/api/trades"
        body

        jsonSerialize
            {| stockId = 1
               side = "BUY"
               quantity = 10
               price = 175.50
               executedAt = "2026-07-02T12:00:00Z" |}
    }
    |> Request.sendAsync
    |> Async.Ignore

// run the shuffled requests
[| 0..100 |]
|> Array.map (fun n ->
    match n with
    | i when i < 25 -> healthReq ()
    | i when i < 50 -> pingReq ()
    | _ -> tradeReq ())
|> Array.randomShuffle
|> Async.Parallel
|> Async.RunSynchronously
