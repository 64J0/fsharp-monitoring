module API.PrometheusMiddleware

open Microsoft.AspNetCore.Http
open Giraffe
open API.Prometheus

let requestCounter: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let endpointCounterName =
                ctx.Request.Path.Value + "_counter" |> (fun s -> s.Replace("/", "_").Replace(".", "_").[1..])

            let endpointCounterDescription =
                sprintf "Endpoint request counter. Path: %s" ctx.Request.Path.Value

            let endpointCounter = createCounter endpointCounterName endpointCounterDescription

            endpointCounter.Inc()

            return! next ctx
        }
