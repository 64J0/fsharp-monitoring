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

            // Skip the remaining pipeline
            //
            // WARNING!!!!
            //
            // We need to return `Some ctx` instead of `None` here, otherwise, when
            // reaching the **handleResult** (https://github.com/giraffe-fsharp/Giraffe/blob/master/src/Giraffe/EndpointRouting.fs#L101-L104)
            // Giraffe will try to set the response status code for a request that
            // was already finished. This will throw some exception that one can
            // finds at the server logs.
            //
            // Something that I still don't fully understand is why this exception
            // related to the status code change does not happen for all the cases.
            // When testing it was only happening when the /api/prediction endpoint
            // fails (i.e., throws an exception itself).
            //
            // To test this error locally, just change this "return Some ctx" to
            // "return None", start the API and use the /api/prediction endpoint with
            // an invalid payload, like:
            //
            // curl -X POST -H "Accept: application/json" -d '{"id":"1", "crimesPerCapta":0.01}' localhost:8088/api/prediction
            //
            // P.S.: Notice that the id is a string instead of an integer ^.
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
