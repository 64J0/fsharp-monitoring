module Env

open System

let private setEnvVarDefaultValue (defaultValue: string) (readEnvVar: string) =
    match readEnvVar with
    | null -> defaultValue
    | _ -> readEnvVar

let HOST =
    Environment.GetEnvironmentVariable "HOST"
    |> setEnvVarDefaultValue "http://localhost"

let PORT = Environment.GetEnvironmentVariable "PORT" |> setEnvVarDefaultValue "8085"

let MIN_LOG_LEVEL =
    Environment.GetEnvironmentVariable "MIN_LOG_LEVEL"
    |> setEnvVarDefaultValue "DEBUG"

let DB_CONNECTION_STRING =
    Environment.GetEnvironmentVariable "DB_CONNECTION_STRING"
    |> setEnvVarDefaultValue "Host=localhost;Database=stocks_db;Username=stocks_user;Password=stocks_pass;Port=5432"
