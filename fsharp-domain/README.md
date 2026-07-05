# Domain Layer

The **domain layer** is the innermost ring of clean architecture. It contains the core business concepts of the system expressed entirely in F# types, with **no dependencies on any framework, database, or external library**.

## What lives here

| Folder          | Content                                                                                    |
| --------------- | ------------------------------------------------------------------------------------------ |
| `Entities/`     | Records representing the main business objects (`Stock`, `Quote`, `Trade`)                 |
| `ValueObjects/` | Strongly-typed wrappers that enforce invariants at construction time (`Ticker`, `Money`)   |
| `Errors/`       | A discriminated union of all possible domain errors (`DomainError`)                        |
| `Rules/`        | Pure functions that express **business rules** (e.g., trade validation)                    |

## Design principles

* **Value objects** replace primitive types wherever the domain attaches meaning or constraints. `Ticker.create "AAPL"` returns `Result<Ticker, DomainError>`, guaranteeing only valid tickers exist at runtime.
* **Entities** are immutable F# records. Identity is modelled with single-case discriminated unions (`StockId`, `QuoteId`, `TradeId`) to prevent accidental type confusion.
* **No IO** — everything in this layer is a pure function or a data declaration. Tests need no mocks or fakes.
