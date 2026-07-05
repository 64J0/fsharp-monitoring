# Repositories (Adapters)

**Repositories** are the concrete implementations of the application layer's `IXxxRepository` port interfaces. They live in the infrastructure layer because they depend on a specific database technology (SqlHydra + PostgreSQL).

## Pattern

Each repository class:

1. Receives a `QueryContextFactory` (a SqlHydra singleton that manages `NpgsqlDataSource`) via **constructor injection**.
2. Implements the corresponding port interface (e.g., `IStockRepository`).
3. Uses SqlHydra's CE query DSL (`selectTask`, `insertTask`) for type-safe SQL.
4. Delegates row-to-domain mapping to the corresponding mapper in `Mappers/`.
5. Wraps errors in `DatabaseError` so callers only see `DomainError` values.

```
IStockRepository (port, in fsharp-application)
    ↑ implemented by
StockRepository (adapter, here)
    → uses QueryContextFactory → SqlHydra CE → PostgreSQL
    → uses StockMapper → Domain.Stock
```

## Adapter pattern

The repository adapter pattern decouples the *what* (defined in the application port) from the *how* (the SQL queries and ORM specifics defined here). This is an application of the **Adapter** pattern from the *Gang of Four*.

## Dependency injection

All repository types are registered in `fsharp-api/Program.fs` as **scoped** services:

```fsharp
services.AddScoped<IStockRepository>(fun sp ->
    StockRepository(sp.GetRequiredService<QueryContextFactory>()))
```

They are resolved by ASP.NET's DI container and injected into use case handlers automatically.
