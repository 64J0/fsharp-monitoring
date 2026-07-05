module FsharpAPI.Domain.Entities.Quote

open System
open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Domain.Entities.Stock

type QuoteId = QuoteId of int64

/// Represents a market quote (price snapshot) for a stock at a given instant.
type Quote =
    { Id: QuoteId
      StockId: StockId
      Price: Money
      Volume: int64
      QuotedAt: DateTimeOffset }
