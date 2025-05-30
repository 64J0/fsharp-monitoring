#r "nuget: FsHttp, 15.0.1"

open FsHttp

Fsi.disableDebugLogs ()

let healthReq () =
    http { GET "http://localhost:8085/health" } |> Request.sendAsync |> Async.Ignore

let pingReq () =
    http { GET "http://localhost:8085/ping/foo" }
    |> Request.sendAsync
    |> Async.Ignore

let predictionReq () =
    http {
        POST "http://localhost:8085/api/prediction"
        body
        jsonSerialize {| id = 1; crimesPerCapta = 0.01 |}
    }
    |> Request.sendAsync
    |> Async.Ignore

let predictionFailReq () =
    http {
        POST "http://localhost:8085/api/prediction"
        body
        jsonSerialize {| id = "1"; crimesPerCapta = 0.01 |}
    }
    |> Request.sendAsync
    |> Async.Ignore

// run the shuffled requests
[| 0..100 |]
|> Array.map (fun n ->
    match n with
    | i when i < 25 -> healthReq ()
    | i when i < 50 -> pingReq ()
    | i when i < 75 -> predictionReq ()
    | _ -> predictionFailReq ())
|> Array.randomShuffle
|> Async.Parallel
|> Async.RunSynchronously
