module API.Controller

open Saturn
open Prometheus

let indexCounter =
    Metrics.CreateCounter(
        "index_endpoint_total", 
        "The total number of times the index endpoint was called", CounterConfiguration()
    )

let indexFn (ctx) =
    indexCounter.Inc() |> ignore

    "Index handler version 1" 
    |> Controller.text ctx

let apiController = controller {
    index (indexFn)
}