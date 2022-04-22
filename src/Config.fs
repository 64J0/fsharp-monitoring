module API.Config

// https://saturnframework.org/reference/Saturn/saturn-application-applicationbuilder.html

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Saturn
open Microsoft.Extensions.Logging
open Prometheus

open API

let loggingConfig (builder: ILoggingBuilder) =
    builder.SetMinimumLevel(LogLevel.Information) 
    |> ignore

// let configureApp (app: IApplicationBuilder) =
//     app.UseRouting()
//     app.UseHttpMetrics()

// https://gist.github.com/pecigonzalo/463ebb7d6f8ed7b8b102f000edb8cf6b#metrics
let configureServices (services: IServiceCollection) =
    // Add our metrics server and default http metrics handlers
    // services.Add

    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore
    services

let serverConfig = application {
    logging loggingConfig
    use_antiforgery
    
    use_router Router.appRouter
    url "http://localhost:8085"
    use_gzip

    // app_config configureApp
    service_config configureServices
}