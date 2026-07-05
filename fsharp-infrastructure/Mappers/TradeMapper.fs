module FsharpAPI.Infrastructure.Mappers.TradeMapper

open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Infrastructure.Database

/// Parses the string column value into the TradeSide discriminated union.
let private parseSide (raw: string) : Result<TradeSide, DomainError> =
    match raw.ToUpperInvariant() with
    | "BUY" -> Ok Buy
    | "SELL" -> Ok Sell
    | other -> Error(InvalidSide $"Unknown trade side stored in DB: '{other}'")

/// Transforms a database row (``public``.trades) into a domain Trade entity.
let toDomain (row: trades) : Result<Trade, DomainError> =
    match parseSide row.side, Money.createPositive row.price with
    | Error e, _ -> Error e
    | _, Error e -> Error e
    | Ok side, Ok price ->
        Ok
            { Id = TradeId row.id
              StockId = StockId row.stock_id
              Side = side
              Quantity = row.quantity
              Price = price
              ExecutedAt = row.executed_at
              CreatedAt = row.created_at }

/// Transforms a domain Trade entity into a database row.
let fromDomain (trade: Trade) : trades =
    let sideStr =
        match trade.Side with
        | Buy -> "BUY"
        | Sell -> "SELL"

    { id = let (TradeId id) = trade.Id in id
      stock_id = let (StockId id) = trade.StockId in id
      side = sideStr
      quantity = trade.Quantity
      price = Money.value trade.Price
      executed_at = trade.ExecutedAt
      created_at = trade.CreatedAt }
