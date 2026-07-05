module FsharpAPI.Domain.Rules.TradeRules

open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Domain.ValueObjects.Money

/// Business rule: trade quantity must be a positive integer.
let validateQuantity (quantity: int64) : Result<int64, DomainError> =
    if quantity <= 0L then
        Error(InvalidQuantity $"Trade quantity must be positive, got: {quantity}")
    else
        Ok quantity

/// Business rule: trade price must be strictly positive.
let validatePrice (price: Money) : Result<Money, DomainError> =
    if Money.value price <= 0m then
        Error(InvalidPrice "Trade price must be positive")
    else
        Ok price

/// Composite validation for a complete trade intent.
let validateTrade (side: TradeSide) (quantity: int64) (price: Money) : Result<unit, DomainError> =
    validateQuantity quantity
    |> Result.bind (fun _ -> validatePrice price)
    |> Result.map (fun _ -> ())
