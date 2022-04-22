module API.Controller

open Microsoft.AspNetCore.Http
open Saturn
open Prometheus

open API.Prometheus

type CreatePayload =
    { id: int
      message: string }

let indexController (ctx: HttpContext) =
    do indexCounter.Inc()
    do indexGauge.Inc(10)

    "Index handler version 1" 
    |> Controller.text ctx

let trackingRandomComputation () =
    let histogram = 
        createHistogram 
            ("create_tracking_random_computation") 
            ("Tracking some random computation being done")
    using (histogram.NewTimer()) (fun _ ->
        let rand = System.Random()
        let rand1 = rand.Next()
        let rand2 = rand.Next()
        let result = rand1 - rand2
        printfn "Result: %A" result
    )
    

let createController (ctx: HttpContext) =
    task {
        let! cnf = Controller.getJson<CreatePayload> ctx

        do createSummary.Observe cnf.id

        let histogram =
            createHistogram 
                ("create_request_id") 
                ("Histogram of requests made by each id")
        do histogram.Observe cnf.id
        do trackingRandomComputation ()

        return! 
            (sprintf "Request OK\nId: %d\nMessage: %s" cnf.id cnf.message)
            |> Controller.text ctx
    }
    

let apiController = controller {
    index (indexController)
    create (createController)
}