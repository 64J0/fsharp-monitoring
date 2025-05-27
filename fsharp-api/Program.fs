open Saturn

open API.Server

[<EntryPoint>]
let main (_args: string[]) =
    do run (serverConfig (Env.HOST) (Env.PORT))
    0
