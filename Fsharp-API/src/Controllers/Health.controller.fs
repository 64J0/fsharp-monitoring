module API.Controller.Health

open Giraffe.Core

/// GET /health endpoint controller.
/// Returns a text showing that the API is healthy.
let index () : HttpHandler =
    setStatusCode 200 >=> text "API instance is healthy!"
