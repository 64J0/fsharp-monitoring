---
description: "Use when writing or modifying HTTP controllers, routing, DI wiring, or Prometheus middleware in the fsharp-api layer. Covers Giraffe handler patterns, DomainError → HTTP mapping, response DTOs, and DI lifetime rules."
applyTo: "fsharp-api/**"
---

# fsharp-api Layer Guidelines

This is the **presentation layer** only. Never put business logic, validation, or domain rules here — those belong in `fsharp-domain/` and `fsharp-application/`.

## Controllers

Each controller file owns one resource (stocks, trades, health). Follow this structure:

**1. Define a response DTO** in the same file:
```fsharp
type StockResponse =
    { Id: int64
      Ticker: string
      // ... primitive types only; no domain types leak out
      CreatedAt: string }  // DateTimeOffset → ISO 8601 string
```

**2. Private `toResponse` mapper** — domain entity → DTO:
```fsharp
let private toResponse (stock: Stock) : StockResponse =
    { Id = let (StockId id) = stock.Id in id   // destructure typed ID
      Ticker = Ticker.value stock.Ticker        // unwrap private DU
      CreatedAt = stock.CreatedAt.ToString("O") }
```

**3. Handler function** returns `HttpHandler`; resolves use-case handler from DI, awaits `Execute`, pattern-matches the `Result`:
```fsharp
let listController () : HttpHandler =
    fun next ctx ->
        task {
            let handler = ctx.RequestServices.GetRequiredService<ListStocksHandler>()
            let! result = handler.Execute()
            return!
                match result with
                | Ok stocks -> json (stocks |> List.map toResponse) next ctx
                | Error err -> (setStatusCode 400 >=> json {| Error = string err |}) next ctx
        }
```

## DomainError → HTTP Status Code Mapping

Map `DomainError` cases to the most semantically correct status code:

| DomainError case | HTTP status |
|---|---|
| `StockNotFound _` | 404 Not Found |
| `InvalidTicker _`, `InvalidPrice _`, `InvalidQuantity _` | 400 Bad Request |
| `DatabaseError _` | 500 Internal Server Error |

Use `setStatusCode` + `json` to compose responses:
```fsharp
| Error (StockNotFound msg) -> (setStatusCode 404 >=> json {| Message = msg |}) next ctx
| Error (DatabaseError _)   -> (setStatusCode 500 >=> json {| Error = "Internal error" |}) next ctx
| Error err                 -> (setStatusCode 400 >=> json {| Error = string err |}) next ctx
```

## Routing (Router.fs)

- Use `Giraffe.EndpointRouting` (`route`, `routef`).
- **Always wrap every route** with Prometheus middleware via Kleisli composition:
  ```fsharp
  route "/api/stocks" (requestCounter >=> requestDuration >=> Stock.listController ())
  ```
- Dynamic segments: `routef "/api/stocks/%s" (fun ticker -> ... ticker)`

## DI Lifetimes (Program.fs)

| Component | Lifetime | Reason |
|---|---|---|
| `QueryContextFactory` | **Singleton** | Wraps `NpgsqlDataSource`; connection pooling managed by Npgsql |
| Repository implementations | **Scoped** | Fresh `QueryContext` per request |
| Use-case handlers | **Scoped** | Receive scoped repos by constructor |

Register repositories via the port interface, not the concrete type:
```fsharp
services.AddScoped<IStockRepository>(fun sp ->
    StockRepository(sp.GetRequiredService<QueryContextFactory>()))
```

## Prometheus Middleware

- `requestCounter` and `requestDuration` are defined in `Router/Prometheus.middleware.fs`.
- Both call `next ctx` first, then record metrics **after** the response — do not reorder.
- Do not create new `Histogram`/`Counter` inside controller functions; keep metrics in the middleware.
