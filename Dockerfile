FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution file
COPY fsharp-monitoring.slnx .

# Copy project files first for layer-cached restore
COPY fsharp-domain/*.fsproj          ./fsharp-domain/
COPY fsharp-application/*.fsproj     ./fsharp-application/
COPY fsharp-infrastructure/*.fsproj  ./fsharp-infrastructure/
COPY fsharp-api/*.fsproj             ./fsharp-api/
COPY db-migrations/*.fsproj          ./db-migrations/
COPY load-test/*.fsproj              ./load-test/

RUN dotnet restore fsharp-api/FsharpAPI.fsproj
RUN dotnet restore db-migrations/FsharpAPI.Migrations.fsproj

# Copy all source code
COPY fsharp-domain/          ./fsharp-domain/
COPY fsharp-application/     ./fsharp-application/
COPY fsharp-infrastructure/  ./fsharp-infrastructure/
COPY fsharp-api/             ./fsharp-api/
COPY db-migrations/          ./db-migrations/

RUN dotnet publish fsharp-api/FsharpAPI.fsproj -c Release -o ./out/api --no-restore
RUN dotnet publish db-migrations/FsharpAPI.Migrations.fsproj -c Release -o ./out/migrations --no-restore

# ── Migrations stage ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS migrations

LABEL org.opencontainers.image.source=https://github.com/64J0/fsharp-monitoring

WORKDIR /app
COPY --from=build /app/out/migrations .

ENTRYPOINT ["/app/FsharpAPI.Migrations"]

# ── API runtime stage ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime

LABEL org.opencontainers.image.source=https://github.com/64J0/fsharp-monitoring

WORKDIR /app
COPY --from=build /app/out/api .

# HTTP API port
EXPOSE 8085
# Prometheus metrics port (do not expose publicly)
EXPOSE 9085

ENTRYPOINT ["/app/FsharpAPI"]
