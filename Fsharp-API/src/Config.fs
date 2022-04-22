module API.Config

// https://saturnframework.org/reference/Saturn/saturn-application-applicationbuilder.html

open Microsoft.AspNetCore.Builder
open Saturn
open Microsoft.Extensions.Logging
open Prometheus

open API

let loggingConfig (builder: ILoggingBuilder) =
    builder.SetMinimumLevel(LogLevel.Information) 
    |> ignore

// https://gist.github.com/pecigonzalo/463ebb7d6f8ed7b8b102f000edb8cf6b#metrics
let configureApp (app: IApplicationBuilder) =
    app.UseHttpMetrics() |> ignore
    app.UseMetricServer() |> ignore
    app

let serverConfig = application {
    logging loggingConfig
    use_antiforgery
    
    use_router Router.appRouter
    url "http://localhost:8085"
    use_gzip

    app_config configureApp
}