module FsharpAPI.Domain.ValueObjects.Ticker

open FsharpAPI.Domain.Errors

/// Strongly-typed value object representing a stock ticker symbol.
/// Construction is restricted to ensure only valid tickers are created.
type Ticker = private Ticker of string

module Ticker =
    let create (value: string) : Result<Ticker, DomainError> =
        if System.String.IsNullOrWhiteSpace(value) then
            Error(InvalidTicker "Ticker cannot be empty")
        elif value.Length > 10 then
            Error(InvalidTicker $"Ticker '{value}' exceeds maximum length of 10 characters")
        else
            Ok(Ticker(value.ToUpperInvariant()))

    let value (Ticker t) = t
