# Ports (Repository Interfaces)

**Ports** are the contracts the application layer defines for external resources. They express *what* the application needs, not *how* it is provided.

## Why this matters

Without ports, use cases would import database libraries directly, making them impossible to test without a live database and impossible to swap implementations. Ports enable:

* **Testability** — unit tests supply stub implementations (`StubStockRepository`) without touching a database.
* **Replaceability** — the database engine, ORM, or storage medium can change without modifying a single use case.
* **Separation of concerns** — business logic is kept free of infrastructure noise.

## Naming convention

Each file declares a single F# interface type named `I<Entity>Repository`. The infrastructure project's `Repositories/` folder provides the concrete adapter implementing each interface.
