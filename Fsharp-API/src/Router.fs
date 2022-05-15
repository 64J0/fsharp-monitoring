module API.Router

// https://saturnframework.org/explanations/routing.html

open Saturn
open Giraffe.Core

open API

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
    not_found_handler (setStatusCode 404 >=> text "Api 404")
    pipe_through apiPipeline

    forward ("/prediction") (Controller.Prediction.apiController)
}

let appRouter = router {
    pipe_through customMiddleware

    get ("/test") (Controller.Test.index ())
    forward "/api" apiRouter
}