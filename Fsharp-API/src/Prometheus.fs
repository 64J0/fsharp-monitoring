module API.Prometheus

open System.Collections
open Prometheus

/// This function is used to create a counter so we can keep track of how many
/// times some operation is done.
let createCounter (name: string) (description: string) =
    Metrics.CreateCounter(name, description, CounterConfiguration())

/// This function is used to create a new gauge metric.
let createGauge (name: string) (description: string) =
    Metrics.CreateGauge(name, description, GaugeConfiguration())

/// Create a summary metric with a fixed configuration.
/// https://github.com/prometheus-net/prometheus-net#summary
/// Summaries track the trends in events over time (10 minutes by default).
let createSummary (name: string) (description: string) =
    let objectives =
        seq {
            new QuantileEpsilonPair(0.5, 0.05)
            new QuantileEpsilonPair(0.9, 0.05)
            new QuantileEpsilonPair(0.95, 0.01)
            new QuantileEpsilonPair(0.99, 0.005)
        }
        |> Immutable.ImmutableList.ToImmutableList

    Metrics.CreateSummary(name, description, new SummaryConfiguration(Objectives = objectives))

/// Create a histogram metric with a fixed configuration
/// https://github.com/prometheus-net/prometheus-net#histogram
/// Histograms track the size and number of events in buckets. This allows for
/// aggregatable calculation of quantiles.
let createHistogram (name: string) (description: string) =
    Metrics.CreateHistogram(name, description, HistogramConfiguration())

/// Track the time consumed in a specific computation based on the histogram
/// metric.
/// https://github.com/prometheus-net/prometheus-net#measuring-operation-duration
let trackComputationHistogram (histogram: Histogram) (computation: float -> float) (data: float) =
    using (histogram.NewTimer()) (fun _ -> computation data)
