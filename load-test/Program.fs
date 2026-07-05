open System
open System.Net.Http
open System.Net.Mime
open System.Text
open System.Text.Json
open NBomber.FSharp
open NBomber.Http
open NBomber.Http.FSharp
open NBomber.Contracts
open NBomber.Plugins.Network.Ping

open API.Controller.Trade // RecordTradePayload

[<Literal>]
let private baseUrl = "http://localhost:8085"

// Stocks seeded by 001_InitialSchema.sql — IDs are assigned in insertion order.
let private tickers =
    [| "AAPL"; "MSFT"; "GOOGL"; "AMZN"; "NVDA"; "JPM"; "GS"; "TSLA" |]

let private stockIds = [| 1L; 2L; 3L; 4L; 5L; 6L; 7L; 8L |]
let private sides = [| "BUY"; "SELL" |]

// Matches Giraffe's JsonNamingPolicy.CamelCase serialiser — must use camelCase keys (e.g. stockId, not StockId).
let private jsonOptions =
    JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

/// Ramp up to `rate` req/s, sustain, then ramp back to zero — 3 minutes total.
let private rampLoad rate =
    [ RampingInject(rate = rate, interval = TimeSpan.FromSeconds 1.0, during = TimeSpan.FromMinutes 1.0)
      Inject(rate = rate, interval = TimeSpan.FromSeconds 1.0, during = TimeSpan.FromMinutes 1.0)
      RampingInject(rate = 0, interval = TimeSpan.FromSeconds 1.0, during = TimeSpan.FromMinutes 1.0) ]

module LoadTest =
    let run () =
        // HttpClient is thread-safe; share a single instance across all scenarios.
        use httpClient = new HttpClient()

        let healthScenario =
            Scenario.create (
                "health",
                fun _ ->
                    task { return! Http.send httpClient (Http.createRequest HttpMethod.Get.Method $"{baseUrl}/health") }
            )
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations (rampLoad 20)

        let listStocksScenario =
            Scenario.create (
                "list_stocks",
                fun _ ->
                    task {
                        let request =
                            Http.createRequest HttpMethod.Get.Method $"{baseUrl}/api/stocks"
                            |> Http.withHeader "Accept" MediaTypeNames.Application.Json

                        return! Http.send httpClient request
                    }
            )
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations (rampLoad 30)

        let getStockByTickerScenario =
            Scenario.create (
                "get_stock_by_ticker",
                fun _ ->
                    task {
                        let ticker = tickers[Random.Shared.Next tickers.Length]

                        let request =
                            Http.createRequest HttpMethod.Get.Method $"{baseUrl}/api/stocks/{ticker}"
                            |> Http.withHeader "Accept" MediaTypeNames.Application.Json

                        return! Http.send httpClient request
                    }
            )
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations (rampLoad 50)

        let recordTradeScenario =
            Scenario.create (
                "record_trade",
                fun _ ->
                    task {
                        // Using the API's own DTO ensures this breaks at compile time if the payload shape changes.
                        let payload: RecordTradePayload =
                            { StockId = stockIds[Random.Shared.Next stockIds.Length]
                              Side = sides[Random.Shared.Next sides.Length]
                              Quantity = Random.Shared.Next(1, 1000) |> int64
                              Price = Math.Round(decimal (Random.Shared.NextDouble() * 999.0 + 1.0), 2)
                              ExecutedAt = DateTimeOffset.UtcNow }

                        // Explicit HttpContent annotation avoids implicit-widening issues with Http.withBody.
                        let body: HttpContent =
                            new StringContent(
                                JsonSerializer.Serialize(payload, jsonOptions),
                                Encoding.UTF8,
                                MediaTypeNames.Application.Json
                            )

                        let request =
                            Http.createRequest HttpMethod.Post.Method $"{baseUrl}/api/trades"
                            |> Http.withBody body

                        return! Http.send httpClient request
                    }
            )
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations (rampLoad 10)

        NBomberRunner.registerScenarios
            [ healthScenario
              listStocksScenario
              getStockByTickerScenario
              recordTradeScenario ]
        |> NBomberRunner.withWorkerPlugins
            [ new PingPlugin(PingPluginConfig.createDefault "localhost")
              new HttpMetricsPlugin(seq { HttpVersion.Version1 }) ]
        |> NBomberRunner.run

type ReturnCode =
    | Success
    | Failure

    member this.ToInt() =
        match this with
        | Success -> 0
        | Failure -> 1

[<EntryPoint>]
let main (_args: string array) =
    try
        let _ = LoadTest.run ()
        ReturnCode.Success.ToInt()
    with ex ->
        printfn "An error occurred when running the load test: %s" ex.Message
        ReturnCode.Failure.ToInt()
