module FsharpAPI.Application.Ports.ITradeRepository

open System.Threading.Tasks
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Trade

/// Port for recording and querying trade executions.
type ITradeRepository =
    abstract member GetByStockId: StockId -> Task<Result<Trade list, DomainError>>
    abstract member Add: Trade -> Task<Result<Trade, DomainError>>
