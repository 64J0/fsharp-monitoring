module FsharpAPI.Infrastructure.Repositories.StockRepository

open System.Threading.Tasks
open SqlHydra.Query
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.ValueObjects.Ticker
open FsharpAPI.Application.Ports.IStockRepository
open FsharpAPI.Infrastructure.Database
open FsharpAPI.Infrastructure.Mappers.StockMapper

/// Concrete adapter implementing IStockRepository using SqlHydra + PostgreSQL.
/// Receives a QueryContextFactory (singleton) via constructor injection.
type StockRepository(db: QueryContextFactory) =

    interface IStockRepository with

        member _.GetByTicker(ticker: Ticker) : Task<Result<Stock option, DomainError>> =
            task {
                let tickerVal = Ticker.value ticker

                let! row =
                    selectTask db {
                        for s in stocks do
                            where (s.ticker = tickerVal)
                            select s
                            tryHead
                    }

                return
                    match row with
                    | None -> Ok None
                    | Some r -> toDomain r |> Result.map Some
            }

        member _.GetById(StockId id) : Task<Result<Stock option, DomainError>> =
            task {
                let! row =
                    selectTask db {
                        for s in stocks do
                            where (s.id = id)
                            select s
                            tryHead
                    }

                return
                    match row with
                    | None -> Ok None
                    | Some r -> toDomain r |> Result.map Some
            }

        member _.ListAll() : Task<Result<Stock list, DomainError>> =
            task {
                let! rows =
                    selectTask db {
                        for s in stocks do
                            orderBy s.ticker
                            select s
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
                                | Ok s -> Some s
                                | _ -> None)
                        )
                    else
                        Error(DatabaseError $"Failed to map {errors.Length} stock record(s)")
            }

        member _.Add(stock: Stock) : Task<Result<Stock, DomainError>> =
            task {
                try
                    let row = fromDomain stock

                    let! newId =
                        insertTask db {
                            for s in stocks do

                                entity
                                    { id = 0L
                                      ticker = row.ticker
                                      company_name = row.company_name
                                      sector = row.sector
                                      exchange = row.exchange
                                      created_at = row.created_at }

                                getId s.id
                        }

                    return Ok { stock with Id = StockId(int64 newId) }
                with ex ->
                    return Error(DatabaseError $"Failed to insert stock: {ex.Message}")
            }
