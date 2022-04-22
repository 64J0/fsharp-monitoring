FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy main project
COPY Fsharp-API/ .

RUN dotnet publish -c Release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8085
ENTRYPOINT ["dotnet", "FsharpAPI.dll"]