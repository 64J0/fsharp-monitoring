open System.Net
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

open Giraffe
open Giraffe.EndpointRouting
open Prometheus

open API.PrometheusMiddleware
open API.Router

let PROMETHEUS_PORT: uint16 = 9085us

let notFoundHandler =
    requestCounter
    >=> requestDuration
    >=> (int HttpStatusCode.NotFound |> setStatusCode)
    >=> json {| Message = "Route not Found" |}

let private configureLogging (loggingBuilder: ILoggingBuilder) =
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging#logging-providers
    match Env.MIN_LOG_LEVEL with
    | "DEBUG" -> LogLevel.Debug
    | _ -> LogLevel.Information
    |> loggingBuilder.ClearProviders().AddConsole().SetMinimumLevel
    |> ignore

let private configureServices (services: IServiceCollection) =
    services
        .AddMetricServer(fun (options: KestrelMetricServerOptions) -> options.Port <- PROMETHEUS_PORT)
        .AddRouting()
        .AddGiraffe()
    |> ignore

let private configureApp (appBuilder: IApplicationBuilder) =
    appBuilder.UseRouting().UseEndpoints(_.MapGiraffeEndpoints(appRouter)).UseGiraffe(notFoundHandler)

[<EntryPoint>]
let main (args: string[]) =
    let builder = WebApplication.CreateBuilder(args)
    configureLogging builder.Logging
    configureServices builder.Services

    let app = builder.Build()

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplication#working-with-ports
    app.Urls.Add($"{Env.HOST}:{Env.PORT}")

    configureApp app
    app.Run()

    0
