module FsharpAPI.Application.UseCases.GetLatestQuote

open System.Threading.Tasks
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Quote
open FsharpAPI.Application.Ports.IQuoteRepository

type GetLatestQuoteQuery = { StockId: int64 }

/// Retrieves the most recent market quote recorded for a given stock.
type GetLatestQuoteHandler(quoteRepository: IQuoteRepository) =
    member _.Execute(query: GetLatestQuoteQuery) : Task<Result<Quote option, DomainError>> =
        quoteRepository.GetLatestByStockId(StockId query.StockId)
