module API.Controller.Health

open System.Net

open Giraffe.Core

/// GET /health endpoint controller.
/// Returns a text showing that the API is healthy.
let index () : HttpHandler =
    int HttpStatusCode.OK |> setStatusCode
    >=> json {| Message = "API instance is healthy!" |}
