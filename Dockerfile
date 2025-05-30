FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# copy main project
COPY fsharp-api/ .

RUN dotnet publish -c Release -o ./bin

# final stage/image
FROM mcr.microsoft.com/dotnet/nightly/aspnet:9.0-noble-chiseled AS runtime

LABEL org.opencontainers.image.source=https://github.com/64J0/fsharp-monitoring

WORKDIR /app

COPY --from=build /app/bin .

# normal server
EXPOSE 8085

# Prometheus metrics server
# must not be exposed publicly
EXPOSE 9085

ENTRYPOINT ["/app/FsharpAPI"]
