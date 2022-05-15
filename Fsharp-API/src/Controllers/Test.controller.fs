module API.Controller.Test

open Giraffe.Core

open API.Prometheus

let index () =
    do testIndexCounter.Inc()

    text "Test endpoint is working."