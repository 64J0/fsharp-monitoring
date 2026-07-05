module FsharpAPI.Infrastructure.Mappers.QuoteMapper

open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Quote
open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Infrastructure.Database

/// Transforms a database row (``public``.quotes) into a domain Quote entity.
let toDomain (row: quotes) : Result<Quote, DomainError> =
    match Money.create row.price with
    | Error e -> Error e
    | Ok price ->
        Ok
            { Id = QuoteId row.id
              StockId = StockId row.stock_id
              Price = price
              Volume = row.volume
              QuotedAt = row.quoted_at }

/// Transforms a domain Quote entity into a database row.
let fromDomain (quote: Quote) : quotes =
    { id = let (QuoteId id) = quote.Id in id
      stock_id = let (StockId id) = quote.StockId in id
      price = Money.value quote.Price
      volume = quote.Volume
      quoted_at = quote.QuotedAt }
