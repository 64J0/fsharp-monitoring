module API.Controller

open Saturn
open Prometheus

// Counter for the index endpoint
let indexCounter =
    Metrics.CreateCounter(
        "index_endpoint_total", 
        "The total number of times the index endpoint was called", 
        CounterConfiguration()
    )

let indexController (ctx) =
    indexCounter.Inc() |> ignore

    "Index handler version 1" 
    |> Controller.text ctx

let apiController = controller {
    index (indexController)
}