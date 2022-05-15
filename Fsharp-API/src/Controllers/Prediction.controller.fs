module API.Controller.Prediction

open Microsoft.AspNetCore.Http
open Saturn
open Prometheus

open API.Prometheus
open API.DataScience

type CreatePayload =
    { id: int
      crimesPerCapta: float }

let trackPredictionComputation (crimesPerCapta: float) =
    let histogram = 
        createHistogram 
            ("create_tracking_random_computation") 
            ("Tracking some random computation being done")
            
    using (histogram.NewTimer()) (fun _ ->
        crimesPerCapta 
        |> getPrediction
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
        
        let prediction = trackPredictionComputation (cnf.crimesPerCapta)

        return! 
            (sprintf "Request OK\nId: %d\nCrimesPerCapta: %f\nPrice Prediction: %f" (cnf.id) (cnf.crimesPerCapta) (prediction))
            |> Controller.text ctx
    }
    

let apiController = controller {
    create (createController)
}