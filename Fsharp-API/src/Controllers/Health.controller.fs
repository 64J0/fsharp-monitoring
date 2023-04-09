module API.Controller.Health

open Giraffe.Core

/// GET /health endpoint controller.
let index () = text "API instance is healthy!"
