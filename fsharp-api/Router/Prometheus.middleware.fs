module API.PrometheusMiddleware

open Microsoft.AspNetCore.Http

open Giraffe
open Prometheus

open API.MonitoringPrometheus

let requestCounter: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let counterName = "api_request_count_total"
            let counterDescription = "API generic request counter."
            let counterLabelNames = [| "endpoint"; "method"; "status_code" |]

            let requestCounter =
                createCounter (counterName) (counterDescription) (counterLabelNames)

            let! _ = next ctx

            let endpoint = ctx.Request.Path.Value
            let method = ctx.Request.Method
            let statusCode = ctx.Response.StatusCode |> string

            requestCounter.WithLabels(endpoint, method, statusCode).Inc()

            // Check this discussion for why we can't use return None:
            // - https://github.com/giraffe-fsharp/Giraffe/discussions/659
            return Some ctx
        }

let requestDuration: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let histogramName = "api_request_duration_seconds"
            let histogramDescription = "API generic request duration in seconds."
            let histogramLabelNames = [| "endpoint"; "method" |]

            let histogramRequestDuration =
                createHistogram (histogramName) (histogramDescription) (histogramLabelNames)

            let endpoint = ctx.Request.Path.Value
            let method = ctx.Request.Method

            let histogram = histogramRequestDuration.WithLabels(endpoint, method)

            return! using (histogram.NewTimer()) (fun _ -> task { return! next ctx })
        }
