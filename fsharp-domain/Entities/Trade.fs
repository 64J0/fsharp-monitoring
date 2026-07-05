module FsharpAPI.Domain.Entities.Trade

open System
open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Domain.Entities.Stock

type TradeId = TradeId of int64

/// Discriminated union for the direction of a trade.
type TradeSide =
    | Buy
    | Sell

/// Represents a recorded trade execution against a stock.
type Trade =
    { Id: TradeId
      StockId: StockId
      Side: TradeSide
      Quantity: int64
      Price: Money
      ExecutedAt: DateTimeOffset
      CreatedAt: DateTimeOffset }
