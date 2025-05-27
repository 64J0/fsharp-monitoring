module Env

open System

let private setEnvVarDefaultValue (defaultValue: string) (readEnvVar: string) =
    match readEnvVar with
    | null -> defaultValue
    | _ -> readEnvVar

let HOST =
    Environment.GetEnvironmentVariable "HOST" |> setEnvVarDefaultValue "localhost"

let PORT = Environment.GetEnvironmentVariable "PORT" |> setEnvVarDefaultValue "8085"
