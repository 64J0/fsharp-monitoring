# Database Migrations (DbUp)

This project manages the PostgreSQL schema lifecycle using **DbUp**.

## How it works

DbUp discovers SQL scripts embedded as assembly resources (see `Migrations/*.sql`), tracks which scripts have already been applied in a `SchemaVersions` table it creates automatically, and runs only the new ones — in filename order.

## Running migrations

```bash
# Against the local dev database
make db-migrate

# With an explicit connection string
dotnet run --project db-migrations -- "Host=...;Database=...;Username=...;Password=..."

# Via Docker Compose (runs automatically before the API container starts)
make compose-up
```

## Adding a new migration

1. Create a new SQL file in `Migrations/` following the naming convention:

   ```
   003_YourDescription.sql
   ```

2. Write idempotent SQL (use `IF NOT EXISTS`, `ON CONFLICT DO NOTHING`, etc.).

3. Commit the file — the CI pipeline and Docker startup will apply it automatically.

## Migration tracking

DbUp stores the list of applied scripts in the `schemaversions` table of the target database. To inspect it:

```sql
SELECT * FROM schemaversions ORDER BY applied;
```

## Why DbUp instead of EF Core Migrations?

DbUp keeps migrations as plain SQL files that are version-controlled, reviewable in code review, and executable directly against the database without any ORM toolchain. This aligns with the project's philosophy of keeping the persistence layer explicit and transparent.
