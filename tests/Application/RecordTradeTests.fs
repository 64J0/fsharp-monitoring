module Tests.Application.RecordTradeTests

open System
open System.Threading.Tasks
open Xunit
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Trade
open FsharpAPI.Application.Ports.ITradeRepository
open FsharpAPI.Application.UseCases.RecordTrade

/// In-memory stub repository for unit testing the RecordTrade use case.
type StubTradeRepository() =
    interface ITradeRepository with
        member _.GetByStockId(_) = Task.FromResult(Ok [])

        member _.Add(trade) =
            Task.FromResult(Ok { trade with Id = TradeId 42L })

let private validCommand () : RecordTradeCommand =
    { StockId = 1L
      Side = "BUY"
      Quantity = 100L
      Price = 150.25m
      ExecutedAt = DateTimeOffset.UtcNow }

[<Fact>]
let ``Execute with valid BUY command returns Ok Trade`` () =
    task {
        let repo = StubTradeRepository()
        let handler = RecordTradeHandler(repo)
        let cmd = validCommand ()

        let! result = handler.Execute(cmd)

        match result with
        | Ok trade ->
            Assert.Equal(TradeId 42L, trade.Id)
            Assert.Equal(Buy, trade.Side)
            Assert.Equal(100L, trade.Quantity)
        | Error e -> Assert.Fail($"Expected Ok but got Error: {e}")
    }

[<Fact>]
let ``Execute with valid SELL command returns Ok Trade`` () =
    task {
        let repo = StubTradeRepository()
        let handler = RecordTradeHandler(repo)
        let cmd = { validCommand () with Side = "SELL" }

        let! result = handler.Execute(cmd)

        match result with
        | Ok trade -> Assert.Equal(Sell, trade.Side)
        | Error e -> Assert.Fail($"Expected Ok but got Error: {e}")
    }

[<Fact>]
let ``Execute with lowercase side succeeds`` () =
    task {
        let repo = StubTradeRepository()
        let handler = RecordTradeHandler(repo)
        let cmd = { validCommand () with Side = "buy" }

        let! result = handler.Execute(cmd)
        Assert.True(Result.isOk result)
    }

[<Fact>]
let ``Execute with invalid side returns InvalidSide error`` () =
    task {
        let repo = StubTradeRepository()
        let handler = RecordTradeHandler(repo)
        let cmd = { validCommand () with Side = "HOLD" }

        let! result = handler.Execute(cmd)

        match result with
        | Error(InvalidSide _) -> ()
        | _ -> Assert.Fail("Expected InvalidSide error")
    }

[<Fact>]
let ``Execute with zero quantity returns InvalidQuantity error`` () =
    task {
        let repo = StubTradeRepository()
        let handler = RecordTradeHandler(repo)
        let cmd = { validCommand () with Quantity = 0L }

        let! result = handler.Execute(cmd)

        match result with
        | Error(InvalidQuantity _) -> ()
        | _ -> Assert.Fail("Expected InvalidQuantity error")
    }

[<Fact>]
let ``Execute with negative price returns InvalidPrice error`` () =
    task {
        let repo = StubTradeRepository()
        let handler = RecordTradeHandler(repo)
        let cmd = { validCommand () with Price = -1.00m }

        let! result = handler.Execute(cmd)

        match result with
        | Error(InvalidPrice _) -> ()
        | _ -> Assert.Fail("Expected InvalidPrice error")
    }

[<Fact>]
let ``Execute with zero price returns InvalidPrice error`` () =
    task {
        let repo = StubTradeRepository()
        let handler = RecordTradeHandler(repo)
        let cmd = { validCommand () with Price = 0m }

        let! result = handler.Execute(cmd)

        match result with
        | Error(InvalidPrice _) -> ()
        | _ -> Assert.Fail("Expected InvalidPrice error")
    }
