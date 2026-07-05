module FsharpAPI.Infrastructure.Mappers.StockMapper

open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.ValueObjects.Ticker
open FsharpAPI.Infrastructure.Database

/// Transforms a database row (``public``.stocks) into a domain Stock entity.
/// This is the anti-corruption layer boundary between DB representation and domain model.
let toDomain (row: stocks) : Result<Stock, DomainError> =
    match Ticker.create row.ticker with
    | Error e -> Error e
    | Ok ticker ->
        Ok
            { Id = StockId row.id
              Ticker = ticker
              CompanyName = row.company_name
              Sector = row.sector
              Exchange = row.exchange
              CreatedAt = row.created_at }

/// Transforms a domain Stock entity into a database row for persistence.
let fromDomain (stock: Stock) : stocks =
    { id = let (StockId id) = stock.Id in id
      ticker = Ticker.value stock.Ticker
      company_name = stock.CompanyName
      sector = stock.Sector
      exchange = stock.Exchange
      created_at = stock.CreatedAt }
