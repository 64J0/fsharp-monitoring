open Saturn

open System

open API.Server

let setEnvVarDefaultValue (defaultValue: string) (readEnvVar: string) =
    match readEnvVar with
    | null -> defaultValue
    | _ -> readEnvVar

let HOST: string =
    Environment.GetEnvironmentVariable "HOST" |> setEnvVarDefaultValue "localhost"

let PORT: string =
    Environment.GetEnvironmentVariable "PORT" |> setEnvVarDefaultValue "8085"

[<EntryPoint>]
let main (args: string[]) =
    run (serverConfig (HOST) (PORT))
    0
