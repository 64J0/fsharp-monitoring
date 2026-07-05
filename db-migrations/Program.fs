module FsharpAPI.Migrations.Program

open System
open System.Reflection
open DbUp

[<EntryPoint>]
let main (argv: string[]) =
    let connectionString =
        match argv with
        | [| cs |] -> cs
        | _ ->
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            |> Option.ofObj
            |> Option.defaultValue
                "Host=localhost;Database=stocks_db;Username=stocks_user;Password=stocks_pass;Port=5432"

    printfn "Running migrations against: %s" (connectionString.Split(';').[0])

    EnsureDatabase.For.PostgresqlDatabase(connectionString)

    let upgrader =
        DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransactionPerScript()
            .LogToConsole()
            .Build()

    let result = upgrader.PerformUpgrade()

    if result.Successful then
        printfn "Database migration completed successfully."
        0
    else
        eprintfn "Database migration FAILED: %s" result.Error.Message
        1
