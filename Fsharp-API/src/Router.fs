module API.Router

// https://saturnframework.org/explanations/routing.html

open Saturn
open Giraffe.Core

open API

// https://github.com/SaturnFramework/Saturn/issues/225
// Requests must have the header: Accept: application/json
// curl -H "Accept: application/json" localhost:8085/api/test
let apiPipeline = pipeline {
    plug acceptJson
    set_header "x-pipeline-type" "Api"
}

let apiRouter = router {
    not_found_handler (setStatusCode 404 >=> text "Api 404")
    pipe_through apiPipeline

    get "/test" (text "Test endpoint!")

    forward "" Controller.apiController
}

let appRouter = router {
    forward "/api" apiRouter
}