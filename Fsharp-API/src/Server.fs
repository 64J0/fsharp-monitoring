module API.Server

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Saturn
open Prometheus

open API.Router

let private loggingConfig (builder: ILoggingBuilder) =
    builder.SetMinimumLevel LogLevel.Information |> ignore

// https://www.compositional-it.com/news-blog/dependency-injection-with-asp-net-and-f/
let private configureServices (services: IServiceCollection) = services.AddHttpClient()

let private configureApp (app: IApplicationBuilder) = app.UseHttpMetrics().UseMetricServer()

let serverConfig (host: string) (port: string) =
    application {
        logging loggingConfig
        use_antiforgery
        use_gzip

        use_router appRouter
        url (sprintf "http://%s:%s" host port)

        service_config configureServices
        app_config configureApp
    }
