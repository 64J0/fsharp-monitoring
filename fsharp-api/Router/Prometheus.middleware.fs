module API.PrometheusMiddleware

open Microsoft.AspNetCore.Http

open Giraffe
open Prometheus

/// Giraffe middleware that increments a labelled request counter after each
/// response is written.  Uses prometheus-net's idempotent CreateCounter — safe
/// to call on every request (returns the same Counter instance for the same name).
let requestCounter: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let counter =
                Metrics.CreateCounter(
                    "api_request_count_total",
                    "API generic request counter.",
                    [| "endpoint"; "method"; "status_code" |]
                )

            let! result = next ctx

            let endpoint = ctx.Request.Path.Value
            let method = ctx.Request.Method
            let statusCode = ctx.Response.StatusCode |> string

            counter.WithLabels(endpoint, method, statusCode).Inc()

            // Returning Some ctx (not None) is required — see
            // https://github.com/giraffe-fsharp/Giraffe/discussions/659
            return Some ctx
        }

/// Giraffe middleware that wraps the next handler in a Histogram timer, recording
/// the duration of every request broken down by endpoint and HTTP method.
let requestDuration: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let histogram =
                Metrics
                    .CreateHistogram(
                        "api_request_duration_seconds",
                        "API generic request duration in seconds.",
                        [| "endpoint"; "method" |]
                    )
                    .WithLabels(ctx.Request.Path.Value, ctx.Request.Method)

            return! using (histogram.NewTimer()) (fun _ -> task { return! next ctx })
        }
