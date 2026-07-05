module API.Controller.Stock

open System.Net
open Microsoft.Extensions.DependencyInjection

open Giraffe

open FsharpAPI.Domain.ValueObjects.Ticker
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Application.UseCases.GetStockByTicker
open FsharpAPI.Application.UseCases.ListStocks

// ── Response DTOs ────────────────────────────────────────────────────────────

type StockResponse =
    { Id: int64
      Ticker: string
      CompanyName: string
      Sector: string
      Exchange: string
      CreatedAt: string }

let private toResponse (stock: Stock) : StockResponse =
    { Id = let (StockId id) = stock.Id in id
      Ticker = Ticker.value stock.Ticker
      CompanyName = stock.CompanyName
      Sector = stock.Sector
      Exchange = stock.Exchange
      CreatedAt = stock.CreatedAt.ToString("O") }

// ── Handlers ─────────────────────────────────────────────────────────────────

/// GET /api/stocks — returns all stocks.
let listController () : HttpHandler =
    fun next ctx ->
        task {
            let handler = ctx.RequestServices.GetRequiredService<ListStocksHandler>()
            let! result = handler.Execute()

            return!
                match result with
                | Ok stocks ->
                    (HttpStatusCode.OK |> int |> setStatusCode >=> json (List.map toResponse stocks)) next ctx
                | Error err ->
                    (HttpStatusCode.InternalServerError |> int |> setStatusCode
                     >=> json {| Error = string err |})
                        next
                        ctx
        }

/// GET /api/stocks/:ticker — returns a single stock by ticker symbol.
let getByTickerController (ticker: string) : HttpHandler =
    fun next ctx ->
        task {
            let handler = ctx.RequestServices.GetRequiredService<GetStockByTickerHandler>()
            let! result = handler.Execute({ Ticker = ticker })

            return!
                match result with
                | Ok(Some stock) -> (HttpStatusCode.OK |> int |> setStatusCode >=> json (toResponse stock)) next ctx
                | Ok None ->
                    (HttpStatusCode.NotFound |> int |> setStatusCode
                     >=> json {| Message = $"Stock '{ticker}' not found" |})
                        next
                        ctx
                | Error err ->
                    (HttpStatusCode.BadRequest |> int |> setStatusCode
                     >=> json {| Error = string err |})
                        next
                        ctx
        }
