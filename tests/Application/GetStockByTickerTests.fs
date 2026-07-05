module Tests.Application.GetStockByTickerTests

open System
open System.Threading.Tasks
open Xunit
open FsharpAPI.Domain.Errors
open FsharpAPI.Domain.Entities.Stock
open FsharpAPI.Domain.ValueObjects.Ticker
open FsharpAPI.Application.Ports.IStockRepository
open FsharpAPI.Application.UseCases.GetStockByTicker

/// In-memory stub repository for unit testing without a database.
type StubStockRepository(returnStock: Stock option) =
    interface IStockRepository with
        member _.GetByTicker(_) = Task.FromResult(Ok returnStock)
        member _.GetById(_) = Task.FromResult(Ok returnStock)

        member _.ListAll() =
            Task.FromResult(Ok(returnStock |> Option.toList))

        member _.Add(s) = Task.FromResult(Ok s)

let private makeStock () : Stock =
    let ticker =
        Ticker.create "AAPL"
        |> Result.defaultWith (fun e -> failwith $"Test setup failed: {e}")

    { Id = StockId 1L
      Ticker = ticker
      CompanyName = "Apple Inc."
      Sector = "Technology"
      Exchange = "NASDAQ"
      CreatedAt = DateTimeOffset.UtcNow }

[<Fact>]
let ``Execute returns Ok Some when stock exists`` () =
    task {
        let stock = makeStock ()
        let repo = StubStockRepository(Some stock)
        let handler = GetStockByTickerHandler(repo)

        let! result = handler.Execute({ Ticker = "AAPL" })

        match result with
        | Ok(Some s) -> Assert.Equal(stock, s)
        | Ok None -> Assert.Fail("Expected Some stock but got None")
        | Error e -> Assert.Fail($"Expected Ok but got Error: {e}")
    }

[<Fact>]
let ``Execute returns Ok None when stock does not exist`` () =
    task {
        let repo = StubStockRepository(None)
        let handler = GetStockByTickerHandler(repo)

        let! result = handler.Execute({ Ticker = "AAPL" })

        match result with
        | Ok None -> ()
        | Ok(Some _) -> Assert.Fail("Expected None but got Some")
        | Error e -> Assert.Fail($"Expected Ok but got Error: {e}")
    }

[<Fact>]
let ``Execute returns Error when ticker is invalid`` () =
    task {
        let repo = StubStockRepository(None)
        let handler = GetStockByTickerHandler(repo)

        let! result = handler.Execute({ Ticker = "" })

        match result with
        | Error(InvalidTicker _) -> ()
        | Ok _ -> Assert.Fail("Expected Error but got Ok")
        | Error e -> Assert.Fail($"Expected InvalidTicker error but got: {e}")
    }

[<Fact>]
let ``Execute returns Error when ticker is too long`` () =
    task {
        let repo = StubStockRepository(None)
        let handler = GetStockByTickerHandler(repo)

        let! result = handler.Execute({ Ticker = "TOOLONGTICKER" })

        Assert.True(Result.isError result)
    }
