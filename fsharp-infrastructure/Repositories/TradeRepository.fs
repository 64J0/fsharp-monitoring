module FsharpAPI.Infrastructure.Repositories.TradeRepository

open System.Threading.Tasks
open SqlHydra.Query
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Application.Ports.ITradeRepository
open FsharpAPI.Infrastructure.Database
open FsharpAPI.Infrastructure.Mappers.TradeMapper

/// Concrete adapter implementing ITradeRepository using SqlHydra + PostgreSQL.
type TradeRepository(db: QueryContextFactory) =

    interface ITradeRepository with

        member _.GetByStockId(StockId stockId) : Task<Result<Trade list, DomainError>> =
            task {
                let! rows =
                    selectTask db {
                        for t in trades do
                            where (t.stock_id = stockId)
                            orderByDescending t.executed_at
                            select t
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
                                | Ok t -> Some t
                                | _ -> None)
                        )
                    else
                        Error(DatabaseError $"Failed to map {errors.Length} trade record(s)")
            }

        member _.Add(trade: Trade) : Task<Result<Trade, DomainError>> =
            task {
                try
                    let row = fromDomain trade

                    let! newId =
                        insertTask db {
                            for t in trades do

                                entity
                                    { id = 0L
                                      stock_id = row.stock_id
                                      side = row.side
                                      quantity = row.quantity
                                      price = row.price
                                      executed_at = row.executed_at
                                      created_at = row.created_at }

                                getId t.id
                        }

                    return Ok { trade with Id = TradeId(int64 newId) }
                with ex ->
                    return Error(DatabaseError $"Failed to insert trade: {ex.Message}")
            }
