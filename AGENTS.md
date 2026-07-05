# Agent Instructions — fsharp-monitoring

F# Clean Architecture API with Prometheus/Grafana monitoring. Read the per-layer READMEs for detailed rationale.

## Build & Test

```bash
# Full stack (recommended)
make compose-up       # start Postgres + migrations + API + Prometheus + Grafana
make compose-down

# Local dev
cd fsharp-api && dotnet run        # http://localhost:8085
cd fsharp-api && dotnet watch run  # hot-reload

# Tests
make test             # xUnit v3 + FsCheck property tests

# Database
make db-migrate       # run DbUp migrations (Postgres must be running)
make db-gen-types     # regenerate SqlHydra types after schema changes (run from fsharp-infrastructure/)

# Docker images
make build-api-image
```

## Architecture

Five projects, strict inward-only dependency flow:

```
fsharp-api  →  fsharp-application  →  fsharp-domain
                                 ↑
                    fsharp-infrastructure
```

| Project | Role | Key constraint |
|---|---|---|
| `fsharp-domain/` | Entities, value objects, DomainError, business rules | Zero external dependencies |
| `fsharp-application/` | Use-case handlers, port interfaces (IXxxRepository) | Depends only on domain |
| `fsharp-infrastructure/` | SqlHydra + Npgsql repositories, mappers, generated types | Implements application ports |
| `fsharp-api/` | Giraffe HTTP handlers, Router, DI wiring, Prometheus middleware | Depends on application + infrastructure |
| `db-migrations/` | DbUp SQL migration runner (standalone binary) | Reads `DB_CONNECTION_STRING` |

See: [fsharp-domain/README.md](fsharp-domain/README.md), [fsharp-application/README.md](fsharp-application/README.md), [fsharp-infrastructure/README.md](fsharp-infrastructure/README.md), [db-migrations/README.md](db-migrations/README.md)

## F# Conventions

### Value objects — private DU + `create`/`value` module

```fsharp
type Ticker = private Ticker of string
module Ticker =
    let create (v: string) : Result<Ticker, DomainError> = ...
    let value (Ticker t) = t
```

### Entity IDs — typed single-case DUs

```fsharp
type StockId = StockId of int64   // prevents accidental cross-type confusion
let (StockId id) = stock.Id       // pattern-match to extract
```

### Entities — immutable records; no mutation

### Business rules — pure functions returning `Result<_, DomainError>`, never throw

### Use-case handlers — class with single `Execute` method, wired via ASP.NET DI

```fsharp
type RecordTradeHandler(repo: ITradeRepository) =
    member _.Execute(cmd: RecordTradeCommand) : Task<Result<Trade, DomainError>> = ...
```

### Error handling — `Result<'a, DomainError>` everywhere in domain/application; controllers map errors to HTTP status codes

### Ports — F# `interface` types in `fsharp-application/Ports/` (prefix `I`)

### Mappers — `toDomain` returns `Result` (validates DB data); `fromDomain` is total

See: [fsharp-application/Ports/README.md](fsharp-application/Ports/README.md), [fsharp-application/UseCases/README.md](fsharp-application/UseCases/README.md), [fsharp-infrastructure/Mappers/README.md](fsharp-infrastructure/Mappers/README.md)

## Database

- **Migrations**: SQL files in `db-migrations/Migrations/` applied in filename order by DbUp. Use `IF NOT EXISTS` / `ON CONFLICT DO NOTHING` for idempotency.
- **Generated types**: `fsharp-infrastructure/Generated/StocksDb.fs` — **do not edit**; regenerate with `make db-gen-types` after schema changes.
- **Queries**: SqlHydra computation expression DSL (`selectTask`, `insertTask`) in repositories. See [fsharp-infrastructure/Repositories/README.md](fsharp-infrastructure/Repositories/README.md).
- **`QueryContextFactory`** is registered as a **singleton** in DI; repositories receive it by constructor injection.

## Testing

- Frameworks: **xUnit v3** (Facts/Theories) + **FsCheck** (property tests)
- Scope: Domain and Application layers only — no infrastructure/HTTP tests
- Pattern: stub repositories (`type StubXxxRepository() = interface IXxxRepository with ...`) for handler tests — no database needed
- Location: `tests/Domain/`, `tests/Application/`

## Docker Pitfalls

- The `dotnet publish` steps do **not** use `--no-restore`. Omitting that flag avoids NETSDK1064 errors where transitive packages (e.g., `FastExpressionCompiler` pulled by `SqlHydra.Query`) are not found in the NuGet cache at publish time. The solution-level restore step still runs first to benefit from Docker layer caching when only source files change.
- The solution file (`fsharp-monitoring.slnx`) must be used for restore — not individual projects — to resolve the full transitive dependency graph.
- `fsharp-infrastructure/Generated/StocksDb.fs` is committed and must be kept in sync with the schema; it is not generated at build time.

## Environment Variables

| Variable | Example | Used by |
|---|---|---|
| `DB_CONNECTION_STRING` | `Host=localhost;Database=stocks_db;Username=stocks_user;Password=stocks_pass;Port=5432` | API, migrations |
| `HOST` | `http://0.0.0.0` | API |
| `PORT` | `8085` | API |
| `MIN_LOG_LEVEL` | `INFO` | API |
