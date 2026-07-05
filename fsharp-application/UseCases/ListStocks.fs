module FsharpAPI.Application.UseCases.ListStocks

open System.Threading.Tasks
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Application.Ports.IStockRepository

/// Returns all stocks persisted in the system.
type ListStocksHandler(stockRepository: IStockRepository) =
    member _.Execute() : Task<Result<Stock list, DomainError>> = stockRepository.ListAll()
