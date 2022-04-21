module API.Config

// https://saturnframework.org/reference/Saturn/saturn-application-applicationbuilder.html

open Saturn
open Microsoft.Extensions.Logging

open API

let loggingConfig (builder: ILoggingBuilder) =
    builder.SetMinimumLevel(LogLevel.Information) 
    |> ignore

let serverConfig = application {
    logging (loggingConfig)
    use_antiforgery
    
    use_router Router.appRouter
    url "http://localhost:8085"
    use_gzip
}