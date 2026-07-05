module FsharpAPI.Domain.ValueObjects.Money

open FsharpAPI.Domain.Errors

/// Strongly-typed value object representing a monetary amount (price).
/// Ensures prices are always non-negative.
type Money = private Money of decimal

module Money =
    let create (amount: decimal) : Result<Money, DomainError> =
        if amount < 0m then
            Error(InvalidPrice $"Price cannot be negative: {amount}")
        else
            Ok(Money amount)

    let createPositive (amount: decimal) : Result<Money, DomainError> =
        if amount <= 0m then
            Error(InvalidPrice $"Price must be positive: {amount}")
        else
            Ok(Money amount)

    let value (Money m) = m

    let zero = Money 0m
