module FsharpAPI.Application.Ports.IStockRepository

open System.Threading.Tasks
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.ValueObjects.Ticker

/// Port (interface) that the infrastructure layer must implement to provide
/// persistent access to Stock aggregates. Depends only on domain types.
type IStockRepository =
    abstract member GetByTicker: Ticker -> Task<Result<Stock option, DomainError>>
    abstract member GetById: StockId -> Task<Result<Stock option, DomainError>>
    abstract member ListAll: unit -> Task<Result<Stock list, DomainError>>
    abstract member Add: Stock -> Task<Result<Stock, DomainError>>
