module FsharpAPI.Domain.Entities.Stock

open System
open FsharpAPI.Domain.ValueObjects.Ticker

type StockId = StockId of int64

/// Represents a publicly listed stock/equity.
type Stock =
    { Id: StockId
      Ticker: Ticker
      CompanyName: string
      Sector: string
      Exchange: string
      CreatedAt: DateTimeOffset }
