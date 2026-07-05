open System.Net
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

open Giraffe
open Giraffe.EndpointRouting
open Giraffe.OpenApi
open Prometheus
open Scalar.AspNetCore

open API.PrometheusMiddleware
open API.Router

open FsharpAPI.Infrastructure.Database
open FsharpAPI.Infrastructure.Repositories.StockRepository
open FsharpAPI.Infrastructure.Repositories.QuoteRepository
open FsharpAPI.Infrastructure.Repositories.TradeRepository
open FsharpAPI.Application.Ports.IStockRepository
open FsharpAPI.Application.Ports.IQuoteRepository
open FsharpAPI.Application.Ports.ITradeRepository
open FsharpAPI.Application.UseCases.GetStockByTicker
open FsharpAPI.Application.UseCases.ListStocks
open FsharpAPI.Application.UseCases.RecordTrade
open FsharpAPI.Application.UseCases.GetLatestQuote

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
    // ── Infrastructure: database ────────────────────────────────────────────
    // QueryContextFactory holds a NpgsqlDataSource — safe as singleton.
    let dbFactory = QueryContextFactory.Create(Env.DB_CONNECTION_STRING)
    services.AddSingleton<QueryContextFactory>(dbFactory) |> ignore

    services.AddScoped<IStockRepository>(fun sp -> StockRepository(sp.GetRequiredService<QueryContextFactory>()))
    |> ignore

    services.AddScoped<IQuoteRepository>(fun sp -> QuoteRepository(sp.GetRequiredService<QueryContextFactory>()))
    |> ignore

    services.AddScoped<ITradeRepository>(fun sp -> TradeRepository(sp.GetRequiredService<QueryContextFactory>()))
    |> ignore

    // ── Application: use case handlers ──────────────────────────────────────
    services.AddScoped<GetStockByTickerHandler>() |> ignore
    services.AddScoped<ListStocksHandler>() |> ignore
    services.AddScoped<RecordTradeHandler>() |> ignore
    services.AddScoped<GetLatestQuoteHandler>() |> ignore

    // ── Web framework ────────────────────────────────────────────────────────
    services
        .AddMetricServer(fun (options: KestrelMetricServerOptions) -> options.Port <- PROMETHEUS_PORT)
        .AddRouting()
        .AddGiraffe()
        .AddOpenApi(fun options ->
            // F# transformers handle option types and discriminated unions in the schema.
            options.AddSchemaTransformer<FSharpOptionSchemaTransformer>() |> ignore
            options.AddSchemaTransformer<DiscriminatedUnionSchemaTransformer>() |> ignore
            // Document-level metadata shown in Scalar / Swagger UI.
            options.AddDocumentTransformer(fun doc _ _ ->
                doc.Info.Title <- "Stocks Monitoring API"
                doc.Info.Description <- "REST API for tracking stocks, quotes, and trade executions."
                System.Threading.Tasks.Task.CompletedTask)
            |> ignore)
        .AddOpenApi()
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

    // OpenAPI document: GET /openapi/v1.json
    // Scalar interactive UI: GET /scalar/v1
    app.MapOpenApi() |> ignore
    app.MapScalarApiReference() |> ignore

    configureApp app
    app.Run()

    0
