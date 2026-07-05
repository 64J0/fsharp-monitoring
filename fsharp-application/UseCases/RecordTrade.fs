module FsharpAPI.Application.UseCases.RecordTrade

open System
open System.Threading.Tasks
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Domain.Rules.TradeRules
open FsharpAPI.Application.Ports.ITradeRepository

type RecordTradeCommand =
    { StockId: int64
      Side: string
      Quantity: int64
      Price: decimal
      ExecutedAt: DateTimeOffset }

/// Validates and records a trade execution for a given stock.
/// Enforces trade business rules before persisting via the repository port.
type RecordTradeHandler(tradeRepository: ITradeRepository) =
    member _.Execute(cmd: RecordTradeCommand) : Task<Result<Trade, DomainError>> =
        task {
            let sideResult =
                match cmd.Side.ToUpperInvariant() with
                | "BUY" -> Ok Buy
                | "SELL" -> Ok Sell
                | other -> Error(InvalidSide $"Invalid trade side: '{other}'. Must be BUY or SELL")

            match sideResult with
            | Error e -> return Error e
            | Ok side ->
                match Money.createPositive cmd.Price with
                | Error e -> return Error e
                | Ok price ->
                    match validateTrade side cmd.Quantity price with
                    | Error e -> return Error e
                    | Ok() ->
                        let trade: Trade =
                            { Id = TradeId 0L
                              StockId = StockId cmd.StockId
                              Side = side
                              Quantity = cmd.Quantity
                              Price = price
                              ExecutedAt = cmd.ExecutedAt
                              CreatedAt = DateTimeOffset.UtcNow }

                        return! tradeRepository.Add trade
        }
