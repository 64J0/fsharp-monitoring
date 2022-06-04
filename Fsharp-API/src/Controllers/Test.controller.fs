module API.Controller.Test

open Giraffe.Core

open API.Prometheus

/// This values is used to keep track of how many times the testCounter was called.
let private testCounter =
    let counterName = "test_endpoint_counter"
    let counterDescription = "The quantity of times the test endpoint was called."
    createCounter (counterName) (counterDescription)

/// GET /test endpoint controller.
let index () =
    do testCounter.Inc()

    text "Test endpoint is working."