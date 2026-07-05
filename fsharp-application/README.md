# Application Layer

The **application layer** orchestrates the domain. It defines **what the system can do** (use cases) and **what the system needs from the outside world** (ports).

It depends only on the Domain layer. It has **no knowledge of HTTP, databases, or frameworks**.

## What lives here

| Folder       | Content                                                                           |
| ------------ | --------------------------------------------------------------------------------- |
| `Ports/`     | F# interface types (ports) that the infrastructure layer must implement           |
| `UseCases/`  | Handler classes that implement a single, named business operation                 |

## Use Case

A **use case** (also called an *application service* or *command/query handler*) is a thin orchestration layer that:

1. Validates input and constructs domain value objects.
2. Calls one or more repository ports to read/write data.
3. Applies domain rules (from `fsharp-domain/Rules/`).
4. Returns a `Result<..., DomainError>` to the caller.

Each use case is a class with a single `Execute` method, making it easy to register in ASP.NET's DI container and to test in isolation with stub repositories.

## Port (Repository Interface)

A **port** is an F# interface (abstract type) that declares what the application layer needs without saying how it is implemented. The infrastructure layer provides a concrete **adapter** that satisfies the port contract.

This is the *Dependency Inversion Principle*: the application layer defines the abstraction; the infrastructure layer depends on it, not the other way around.
