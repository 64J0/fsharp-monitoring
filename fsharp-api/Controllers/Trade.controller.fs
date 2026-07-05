module API.Controller.Trade

open System
open System.Net
open Microsoft.Extensions.DependencyInjection

open Giraffe

open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Application.UseCases.RecordTrade

// ── Request / Response DTOs ───────────────────────────────────────────────────

type RecordTradePayload =
    { StockId: int64
      Side: string
      Quantity: int64
      Price: decimal
      ExecutedAt: DateTimeOffset }

type TradeResponse =
    { Id: int64
      StockId: int64
      Side: string
      Quantity: int64
      Price: decimal
      ExecutedAt: string
      CreatedAt: string }

let private toResponse (trade: Trade) : TradeResponse =
    let sideStr =
        match trade.Side with
        | Buy -> "BUY"
        | Sell -> "SELL"

    { Id = let (TradeId id) = trade.Id in id
      StockId = let (StockId id) = trade.StockId in id
      Side = sideStr
      Quantity = trade.Quantity
      Price = Money.value trade.Price
      ExecutedAt = trade.ExecutedAt.ToString("O")
      CreatedAt = trade.CreatedAt.ToString("O") }

// ── Handlers ─────────────────────────────────────────────────────────────────

/// POST /api/trades — records a new trade execution.
let createController () : HttpHandler =
    fun next ctx ->
        task {
            let serializer = ctx.GetJsonSerializer()
            let handler = ctx.RequestServices.GetRequiredService<RecordTradeHandler>()

            try
                let! payload = serializer.DeserializeAsync<RecordTradePayload> ctx.Request.Body

                let cmd: RecordTradeCommand =
                    { StockId = payload.StockId
                      Side = payload.Side
                      Quantity = payload.Quantity
                      Price = payload.Price
                      ExecutedAt = payload.ExecutedAt }

                let! result = handler.Execute(cmd)

                return!
                    match result with
                    | Ok trade -> (HttpStatusCode.Created |> int |> setStatusCode >=> json (toResponse trade)) next ctx
                    | Error err ->
                        (HttpStatusCode.BadRequest |> int |> setStatusCode
                         >=> json {| Error = string err |})
                            next
                            ctx
            with ex ->
                return!
                    (HttpStatusCode.BadRequest |> int |> setStatusCode
                     >=> json {| Error = $"Invalid payload: {ex.Message}" |})
                        next
                        ctx
        }
