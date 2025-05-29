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
                >=> json {| Message = $"Pong from {name}!" |}) ]
      POST [ route "/api/prediction" (requestCounter >=> requestDuration >=> Prediction.createController ()) ] ]
