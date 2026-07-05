module FsharpAPI.Infrastructure.Repositories.QuoteRepository

open System.Threading.Tasks
open SqlHydra.Query
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Quote
open FsharpAPI.Application.Ports.IQuoteRepository
open FsharpAPI.Infrastructure.Database
open FsharpAPI.Infrastructure.Mappers.QuoteMapper

/// Concrete adapter implementing IQuoteRepository using SqlHydra + PostgreSQL.
type QuoteRepository(db: QueryContextFactory) =

    interface IQuoteRepository with

        member _.GetLatestByStockId(StockId stockId) : Task<Result<Quote option, DomainError>> =
            task {
                let! row =
                    selectTask db {
                        for q in quotes do
                            where (q.stock_id = stockId)
                            orderByDescending q.quoted_at
                            select q
                            tryHead
                    }

                return
                    match row with
                    | None -> Ok None
                    | Some r -> toDomain r |> Result.map Some
            }

        member _.GetByStockId(StockId stockId) : Task<Result<Quote list, DomainError>> =
            task {
                let! rows =
                    selectTask db {
                        for q in quotes do
                            where (q.stock_id = stockId)
                            orderByDescending q.quoted_at
                            select q
                    }

                let mapped = rows |> Seq.map toDomain |> Seq.toList

                let errors =
                    mapped
                    |> List.choose (function
                        | Error e -> Some e
                        | _ -> None)

                return
                    if errors.IsEmpty then
                        Ok(
                            mapped
                            |> List.choose (function
                                | Ok q -> Some q
                                | _ -> None)
                        )
                    else
                        Error(DatabaseError $"Failed to map {errors.Length} quote record(s)")
            }

        member _.Add(quote: Quote) : Task<Result<Quote, DomainError>> =
            task {
                try
                    let row = fromDomain quote

                    let! newId =
                        insertTask db {
                            for q in quotes do

                                entity
                                    { id = 0L
                                      stock_id = row.stock_id
                                      price = row.price
                                      volume = row.volume
                                      quoted_at = row.quoted_at }

                                getId q.id
                        }

                    return Ok { quote with Id = QuoteId(int64 newId) }
                with ex ->
                    return Error(DatabaseError $"Failed to insert quote: {ex.Message}")
            }
