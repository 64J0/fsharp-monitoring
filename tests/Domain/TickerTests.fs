module Tests.Domain.TickerTests

open Xunit
open FsharpAPI.Domain.ValueObjects.Ticker

[<Fact>]
let ``Ticker.create with valid symbol returns Ok`` () =
    let result = Ticker.create "AAPL"
    Assert.True(Result.isOk result)

[<Fact>]
let ``Ticker.create normalises to uppercase`` () =
    let result = Ticker.create "aapl"

    match result with
    | Ok ticker -> Assert.Equal("AAPL", Ticker.value ticker)
    | Error e -> Assert.Fail($"Expected Ok but got Error: {e}")

[<Fact>]
let ``Ticker.create with empty string returns Error`` () =
    let result = Ticker.create ""
    Assert.True(Result.isError result)

[<Fact>]
let ``Ticker.create with whitespace string returns Error`` () =
    let result = Ticker.create "   "
    Assert.True(Result.isError result)

[<Fact>]
let ``Ticker.create with symbol longer than 10 chars returns Error`` () =
    let result = Ticker.create "TOOLONGTICKER"
    Assert.True(Result.isError result)

[<Fact>]
let ``Ticker.create with exactly 10 chars returns Ok`` () =
    let result = Ticker.create "ABCDEFGHIJ"
    Assert.True(Result.isOk result)

[<Theory>]
[<InlineData("MSFT")>]
[<InlineData("GOOGL")>]
[<InlineData("JPM")>]
[<InlineData("GS")>]
[<InlineData("NVDA")>]
let ``Ticker.create with known stock symbols succeeds`` (symbol: string) =
    let result = Ticker.create symbol
    Assert.True(Result.isOk result)
