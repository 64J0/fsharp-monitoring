module FsharpAPI.Domain.Errors

/// Represents all possible domain-level errors in the stocks financial system.
type DomainError =
    | InvalidTicker of string
    | InvalidPrice of string
    | InvalidQuantity of string
    | InvalidSide of string
    | StockNotFound of string
    | QuoteNotFound of int64
    | TradeNotFound of int64
    | DatabaseError of string
