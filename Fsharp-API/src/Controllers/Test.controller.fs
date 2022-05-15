module API.Controller.Test

open Prometheus
open Giraffe.Core

open API.Prometheus

let index () =
    do indexCounter.Inc()

    text "Test endpoint work."