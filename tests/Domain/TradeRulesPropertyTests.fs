module Tests.Domain.TradeRulesPropertyTests

open FsCheck
open FsCheck.Xunit
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Domain.Rules.TradeRules

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/// Build a Money from a PositiveInt (always succeeds).
let private moneyOf (PositiveInt n) =
    Money.createPositive (decimal n)
    |> Result.defaultWith (fun _ -> failwith "generator produced non-positive amount")

// ---------------------------------------------------------------------------
// Properties: validateQuantity
// ---------------------------------------------------------------------------

/// Any positive int64 must pass quantity validation.
[<Property>]
let ``validateQuantity accepts any positive quantity`` (qty: PositiveInt) =
    let q = int64 qty.Get
    Result.isOk (validateQuantity q)

/// Any negative int64 must be rejected.
[<Property>]
let ``validateQuantity rejects non-positive quantities`` (qty: NegativeInt) =
    let q = int64 qty.Get
    Result.isError (validateQuantity q)

[<Property>]
let ``validateQuantity rejects zero`` () = Result.isError (validateQuantity 0L)

// ---------------------------------------------------------------------------
// Properties: validateTrade
// ---------------------------------------------------------------------------

/// For any strictly positive quantity and price, a Buy trade must succeed.
[<Property>]
let ``validateTrade accepts any positive quantity and price for Buy`` (qty: PositiveInt) (priceInt: PositiveInt) =
    let q = int64 qty.Get
    let price = moneyOf priceInt
    Result.isOk (validateTrade Buy q price)

/// For any strictly positive quantity and price, a Sell trade must succeed.
[<Property>]
let ``validateTrade accepts any positive quantity and price for Sell`` (qty: PositiveInt) (priceInt: PositiveInt) =
    let q = int64 qty.Get
    let price = moneyOf priceInt
    Result.isOk (validateTrade Sell q price)

/// For any negative quantity, the trade must be rejected regardless of price or side.
[<Property>]
let ``validateTrade rejects negative quantity`` (qty: NegativeInt) (priceInt: PositiveInt) =
    let q = int64 qty.Get
    let price = moneyOf priceInt
    Result.isError (validateTrade Buy q price)
