module API.Controller

open Microsoft.AspNetCore.Http
open System.Collections
open Saturn
open Prometheus

type CreatePayload =
    { id: int
      message: string }

// Counter for the index endpoint
let indexCounter =
    Metrics.CreateCounter(
        "index_counter_endpoint_total", 
        "The total number of times the index endpoint was called.", 
        CounterConfiguration()
    )

// Gauge for the index endpoint
let indexGauge =
    Metrics.CreateGauge(
        "index_gauge_endpoint",
        "Some arbitrary number related to the index endpoint."
    )

// Summary for the index endpoint
// https://github.com/prometheus-net/prometheus-net#summary
let createSummary =
    let objectives =
        seq {
            new QuantileEpsilonPair(0.5, 0.05)
            new QuantileEpsilonPair(0.9, 0.05)
            new QuantileEpsilonPair(0.95, 0.01)
            new QuantileEpsilonPair(0.99, 0.005)
        } |> Immutable.ImmutableList.ToImmutableList

    Metrics.CreateSummary(
        "index_summary_request_size_bytes",
        "Summary of index request sizes (in bytes) over last 10 minutes.",
        new SummaryConfiguration(
            Objectives = objectives
        )
    )

let indexController (ctx: HttpContext) =
    indexCounter.Inc() |> ignore
    indexGauge.Inc(10) |> ignore
    "Index handler version 1" 
    |> Controller.text ctx

let createController (ctx: HttpContext) =
    task {
        let! cnf = Controller.getJson<CreatePayload> ctx
        createSummary.Observe cnf.id

        return! 
            (sprintf "Request OK\nId: %d\nMessage: %s" cnf.id cnf.message)
            |> Controller.text ctx
    }
    

let apiController = controller {
    index (indexController)
    create (createController)
}