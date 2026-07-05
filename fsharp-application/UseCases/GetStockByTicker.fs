module FsharpAPI.Application.UseCases.GetStockByTicker

open System.Threading.Tasks
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.ValueObjects.Ticker
open FsharpAPI.Application.Ports.IStockRepository

type GetStockByTickerQuery = { Ticker: string }

/// Orchestrates the retrieval of a stock by its ticker symbol.
/// Validates the ticker value object before querying the repository.
type GetStockByTickerHandler(stockRepository: IStockRepository) =
    member _.Execute(query: GetStockByTickerQuery) : Task<Result<Stock option, DomainError>> =
        task {
            match Ticker.create query.Ticker with
            | Error e -> return Error e
            | Ok ticker -> return! stockRepository.GetByTicker ticker
        }
