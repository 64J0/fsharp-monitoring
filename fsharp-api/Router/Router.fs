module API.Router

open System.Net

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http

open Giraffe
open Giraffe.EndpointRouting
open Giraffe.OpenApi

open API.Controller
open API.PrometheusMiddleware

let appRouter: Endpoint list =
    [ GET
          [ route "/health" (requestCounter >=> requestDuration >=> Health.index ())
            |> configureEndpoint _.WithTags("Health").WithSummary("Liveness check")
            |> addOpenApiSimple<unit, Health.HealthResponse>

            routef "/ping/%s:name" (fun name ->
                requestCounter
                >=> requestDuration
                >=> (int HttpStatusCode.OK |> setStatusCode)
                >=> json {| Message = $"Pong from {name}!" |})
            |> configureEndpoint _.WithTags("Health").WithSummary("Echo ping")
            |> addOpenApiSimple<string, Health.HealthResponse>

            route "/api/stocks" (requestCounter >=> requestDuration >=> Stock.listController ())
            |> configureEndpoint _.WithTags("Stocks").WithSummary("List all stocks")
            |> addOpenApiSimple<unit, Stock.StockResponse list>

            routef "/api/stocks/%s:ticker" (fun ticker ->
                requestCounter >=> requestDuration >=> Stock.getByTickerController ticker)
            |> configureEndpoint _.WithTags("Stocks").WithSummary("Get stock by ticker symbol")
            |> addOpenApiSimple<string, Stock.StockResponse> ]

      POST
          [ route "/api/trades" (requestCounter >=> requestDuration >=> Trade.createController ())
            |> configureEndpoint _.WithTags("Trades").WithSummary("Record a trade execution")
            |> addOpenApiSimple<Trade.RecordTradePayload, Trade.TradeResponse> ] ]
