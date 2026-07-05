.PHONY: load-test db-migrate test

compose-up:
	docker compose up -d --build
compose-down:
	docker compose down

start-api-container: build-api-image
	docker run -d -e HOST="0.0.0.0" -p 8085:8085 -p 9085:9085 fsharp-api:v1
build-api-image:
	docker build -t fsharp-api:v1 --target runtime .

# Run database migrations (requires PostgreSQL to be running)
db-migrate:
	dotnet run --project ./db-migrations/FsharpAPI.Migrations.fsproj

# Run all unit tests
test:
	dotnet test ./tests/FsharpAPI.Tests.fsproj --logger "console;verbosity=normal"

# Regenerate SqlHydra types from the running database (run db-migrate first)
db-gen-types:
	cd fsharp-infrastructure && dotnet sqlhydra npgsql

request-health:
	curl localhost:8085/health
request-ping:
	curl localhost:8085/ping/foo
request-stocks:
	curl localhost:8085/api/stocks
request-stock-ticker:
	curl localhost:8085/api/stocks/AAPL
request-trade:
	curl -X POST \
		-H "Content-Type: application/json" \
		-d '{"stockId":1,"side":"BUY","quantity":10,"price":175.50,"executedAt":"2026-07-02T12:00:00Z"}' \
		localhost:8085/api/trades
# use this command after running `make compose-up`
load-test:
	dotnet run --project ./load-test/load-test.fsproj