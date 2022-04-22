module API.Prometheus

open System.Collections
open Prometheus

// Counter for the index endpoint
// Counters only increase in value and reset to zero when the process restarts.
let indexCounter =
    Metrics.CreateCounter(
        "index_counter_endpoint_total", 
        "The total number of times the index endpoint was called.", 
        CounterConfiguration()
    )

// Gauge for the index endpoint
// Gauges can have any numeric value and change arbitrarily.
let indexGauge =
    Metrics.CreateGauge(
        "index_gauge_endpoint",
        "Some arbitrary number related to the index endpoint."
    )

// Summary for the index endpoint
// https://github.com/prometheus-net/prometheus-net#summary
// Summaries track the trends in events over time (10 minutes by default).
let createSummary =
    let objectives =
        seq {
            new QuantileEpsilonPair(0.5, 0.05)
            new QuantileEpsilonPair(0.9, 0.05)
            new QuantileEpsilonPair(0.95, 0.01)
            new QuantileEpsilonPair(0.99, 0.005)
        } |> Immutable.ImmutableList.ToImmutableList

    Metrics.CreateSummary(
        "create_summary_request_size_bytes",
        "Summary of index request sizes (in bytes) over last 10 minutes.",
        new SummaryConfiguration(
            Objectives = objectives
        )
    )

// https://github.com/prometheus-net/prometheus-net#histogram
// Histograms track the size and number of events in buckets. This allows for 
// aggregatable calculation of quantiles.
let createHistogram (name: string) (description: string) =
    Metrics.CreateHistogram(
        name,
        description
    )