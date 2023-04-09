module API.Router

open Saturn
open Giraffe.Core

open API.Controller
open API.PrometheusMiddleware

let prometheusPreOperationsMiddleware = pipeline { plug requestCounter }

// https://github.com/SaturnFramework/Saturn/issues/225
// Requests must have the header: Accept: application/json
// curl -H "Accept: application/json" localhost:8085/api/test
let private apiPipeline = pipeline { plug acceptJson }

let private apiRouter =
    router {
        pipe_through apiPipeline

        forward ("/prediction") (Prediction.apiController)
    }

let appRouter =
    router {
        pipe_through prometheusPreOperationsMiddleware
        not_found_handler (setStatusCode 404 >=> text "Endpoint not found.")

        get ("/health") (setStatusCode 200 >=> (Health.index ()))

        forward "/api" apiRouter
    }
