FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /app

# copy main project
COPY Fsharp-API/ .

RUN dotnet publish -c Release -o ./bin

# final stage/image
FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0-jammy-chiseled AS runtime
WORKDIR /app

COPY --from=build /app/bin .

# normal server
EXPOSE 8085
# Prometheus metrics server
EXPOSE 9085

ENTRYPOINT ["/app/FsharpAPI"]
