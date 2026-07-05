module FsharpAPI.Application.Ports.IQuoteRepository

open System.Threading.Tasks
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Quote

/// Port for accessing Quote data. The application layer defines this contract;
/// the infrastructure layer supplies the concrete adapter.
type IQuoteRepository =
    abstract member GetLatestByStockId: StockId -> Task<Result<Quote option, DomainError>>
    abstract member GetByStockId: StockId -> Task<Result<Quote list, DomainError>>
    abstract member Add: Quote -> Task<Result<Quote, DomainError>>
