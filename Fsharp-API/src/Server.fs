module API.Server

// https://saturnframework.org/reference/Saturn/saturn-application-applicationbuilder.html

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Saturn
open Prometheus
open System

open API.Router

let private setEnvVarDefaultValue (defaultValue: string) (readEnvVar: string) =
    match readEnvVar with
    | null -> defaultValue
    | _ -> readEnvVar

let private HOST: string =
    Environment.GetEnvironmentVariable "HOST" |> setEnvVarDefaultValue "localhost"

let private PORT: string =
    Environment.GetEnvironmentVariable "PORT" |> setEnvVarDefaultValue "8085"

let private loggingConfig (builder: ILoggingBuilder) =
    builder.SetMinimumLevel LogLevel.Information |> ignore

// https://www.compositional-it.com/news-blog/dependency-injection-with-asp-net-and-f/
let private configureServices (services : IServiceCollection) =
    services.AddHttpClient()

// https://gist.github.com/pecigonzalo/463ebb7d6f8ed7b8b102f000edb8cf6b#metrics
// TODO: start the metrics server in a different PORT
// this way we can segregate the API server from the metrics server
let private configureApp (app: IApplicationBuilder) = app.UseHttpMetrics().UseMetricServer()

let serverConfig =
    application {
        logging loggingConfig
        use_antiforgery
        use_gzip

        use_router appRouter
        url (sprintf "http://%s:%s" HOST PORT)

        service_config configureServices
        app_config configureApp
    }
