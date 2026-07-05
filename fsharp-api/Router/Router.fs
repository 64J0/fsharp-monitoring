module API.Router

open System.Net

open Giraffe
open Giraffe.EndpointRouting

open API.Controller
open API.PrometheusMiddleware

let appRouter: Endpoint list =
    [ GET
          [ route "/health" (requestCounter >=> requestDuration >=> Health.index ())
            routef "/ping/%s" (fun name ->
                requestCounter
                >=> requestDuration
                >=> (int HttpStatusCode.OK |> setStatusCode)
                >=> json {| Message = $"Pong from {name}!" |})
            route "/api/stocks" (requestCounter >=> requestDuration >=> Stock.listController ())
            routef "/api/stocks/%s" (fun ticker ->
                requestCounter >=> requestDuration >=> Stock.getByTickerController ticker) ]
      POST [ route "/api/trades" (requestCounter >=> requestDuration >=> Trade.createController ()) ] ]
