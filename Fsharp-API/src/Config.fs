module API.Config

// https://saturnframework.org/reference/Saturn/saturn-application-applicationbuilder.html

open Microsoft.AspNetCore.Builder
open Saturn
open Microsoft.Extensions.Logging
open Prometheus

open API

let setEnvVarDefaultValue (defaultValue: string) (readEnvVar: string) =
    match readEnvVar with
    | null -> defaultValue
    | _ -> readEnvVar

let HOST =
    System.Environment.GetEnvironmentVariable("HOST")
    |> setEnvVarDefaultValue ("localhost")

let PORT =
    System.Environment.GetEnvironmentVariable("PORT")
    |> setEnvVarDefaultValue ("8085")

    

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
    url (sprintf "http://%s:%s" HOST PORT)
    use_gzip

    app_config configureApp
}