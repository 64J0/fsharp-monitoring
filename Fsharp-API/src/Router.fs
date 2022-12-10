module API.Router

open Saturn
open Giraffe.Core

open API

// One can find more information about the routing configuration
// of Saturn by consulting the following link:
// https://saturnframework.org/explanations/routing.html

let private customMiddleware = pipeline {
    set_header "x-pipeline-type" "fsharp-api"
}

// https://github.com/SaturnFramework/Saturn/issues/225
// Requests must have the header: Accept: application/json
// curl -H "Accept: application/json" localhost:8085/api/test
let private apiPipeline = pipeline {
    plug acceptJson
}

let private apiRouter = router {
    pipe_through apiPipeline
    
    forward ("/prediction") (Controller.Prediction.apiController)
}

let appRouter = router {
    not_found_handler (setStatusCode 404 >=> text "Endpoint not found.")
    pipe_through customMiddleware

    get ("/health") (setStatusCode 200 >=> (Controller.Health.index ()))
    forward "/api" apiRouter
}