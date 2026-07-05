module API.Controller.Health

open System.Net

open Giraffe.Core

type HealthResponse = { Message: string }

/// GET /health endpoint controller.
/// Returns a text showing that the API is healthy.
let index () : HttpHandler =
    HttpStatusCode.OK |> int |> setStatusCode
    >=> json<HealthResponse> { Message = "API instance is healthy!" }
