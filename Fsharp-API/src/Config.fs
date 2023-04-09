module API.Config

// https://saturnframework.org/reference/Saturn/saturn-application-applicationbuilder.html

open Microsoft.AspNetCore.Builder
open Saturn
open Microsoft.Extensions.Logging
open Prometheus
open System

open API

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

// https://gist.github.com/pecigonzalo/463ebb7d6f8ed7b8b102f000edb8cf6b#metrics
// TODO: start the metrics server in a different PORT
// this way we can segregate the API server from the metrics server
let private configureApp (app: IApplicationBuilder) = app.UseHttpMetrics().UseMetricServer()

let serverConfig =
    application {
        logging loggingConfig
        use_antiforgery

        use_router Router.appRouter
        url (sprintf "http://%s:%s" HOST PORT)
        use_gzip

        app_config configureApp
    }
