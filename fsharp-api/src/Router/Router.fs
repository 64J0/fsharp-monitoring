module API.Router

open Saturn
open Microsoft.AspNetCore.Http
open Giraffe

open API.Controller
open API.PrometheusMiddleware

let private postOpMiddlewareHook (middleware: HttpHandler) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! routerContext = middleware next ctx

            match routerContext with
            | Some routerCtx ->

                let httpContext = DefaultHttpContext()

                httpContext.SetStatusCode routerCtx.Response.StatusCode
                httpContext.Request.Path <- routerCtx.Request.Path
                httpContext.Request.Method <- routerCtx.Request.Method

                let! _ = requestCounter next httpContext

                return! next routerCtx
            | None -> return routerContext
        }

// https://github.com/SaturnFramework/Saturn/issues/225
// Requests must have the header: Accept: application/json
// curl -H "Accept: application/json" localhost:8085/api/test
let private apiPredictionRouter =
    router {
        pipe_through acceptJson

        post "/prediction" (Prediction.createController ())
    }

let appRouter =
    router {
        pipe_through requestDuration

        get ("/health") (postOpMiddlewareHook ((Health.index ())))

        forward ("/api") (postOpMiddlewareHook apiPredictionRouter)

        not_found_handler (postOpMiddlewareHook (setStatusCode 404 >=> text "Endpoint not found."))
    }
