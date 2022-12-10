module API.Controller.Health

open Giraffe.Core

open API.Prometheus

/// This values is used to keep track of how many times the healthCounter was called.
let private healthCounter =
    let counterName = "health_endpoint_counter"
    let counterDescription = "The quantity of times the health endpoint was called."
    createCounter (counterName) (counterDescription)

/// GET /health endpoint controller.
let index () =
    do healthCounter.Inc()

    text "API instance is healthy!"