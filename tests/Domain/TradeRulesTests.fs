module Tests.Domain.TradeRulesTests

open Xunit
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Domain.ValueObjects.Money
open FsharpAPI.Domain.Rules.TradeRules

let private validPrice () =
    Money.createPositive 150.00m
    |> Result.defaultWith (fun _ -> failwith "test setup error")

[<Fact>]
let ``validateQuantity with positive quantity returns Ok`` () =
    let result = validateQuantity 100L
    Assert.True(Result.isOk result)

[<Fact>]
let ``validateQuantity with zero returns Error`` () =
    let result = validateQuantity 0L
    Assert.True(Result.isError result)

[<Fact>]
let ``validateQuantity with negative quantity returns Error`` () =
    let result = validateQuantity -10L
    Assert.True(Result.isError result)

[<Fact>]
let ``validateTrade with valid Buy trade returns Ok`` () =
    let price = validPrice ()
    let result = validateTrade Buy 10L price
    Assert.True(Result.isOk result)

[<Fact>]
let ``validateTrade with valid Sell trade returns Ok`` () =
    let price = validPrice ()
    let result = validateTrade Sell 5L price
    Assert.True(Result.isOk result)

[<Fact>]
let ``validateTrade with zero quantity returns Error`` () =
    let price = validPrice ()
    let result = validateTrade Buy 0L price
    Assert.True(Result.isError result)

[<Fact>]
let ``validateTrade with negative quantity returns Error`` () =
    let price = validPrice ()
    let result = validateTrade Buy -1L price
    Assert.True(Result.isError result)

[<Theory>]
[<InlineData(1L, 0.01)>]
[<InlineData(1000L, 9999.99)>]
[<InlineData(1L, 1.00)>]
let ``validateTrade with valid inputs succeeds`` (qty: int64, priceVal: double) =
    let price =
        Money.createPositive (decimal priceVal)
        |> Result.defaultWith (fun _ -> failwith "test setup error")

    let result = validateTrade Buy qty price
    Assert.True(Result.isOk result)
