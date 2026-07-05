# Use Cases

A **use case** encapsulates a single, named business operation that a user or system can initiate. Each use case class has one public method — `Execute` — making the system's capabilities explicit and discoverable.

## Use cases in this project

| Handler                  | Operation                                              |
| ------------------------ | ------------------------------------------------------ |
| `GetStockByTickerHandler`| Validates a ticker and retrieves the matching stock    |
| `ListStocksHandler`      | Returns all tracked stocks in alphabetical order       |
| `RecordTradeHandler`     | Validates and records a new trade execution            |
| `GetLatestQuoteHandler`  | Retrieves the most recent market quote for a stock     |

## Pattern

```fsharp
type RecordTradeHandler(tradeRepository: ITradeRepository) =
    member _.Execute(cmd: RecordTradeCommand) : Task<Result<Trade, DomainError>> =
        task {
            // 1. Parse / validate input into domain objects
            // 2. Apply domain rules
            // 3. Delegate persistence to the port
        }
```

The constructor receives its dependencies from ASP.NET's DI container. This is the *Dependency Injection* pattern — use cases declare what they need; the composition root (`Program.fs`) wires everything together.
