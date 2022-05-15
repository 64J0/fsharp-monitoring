module API.Controller.Prediction

open Microsoft.AspNetCore.Http
open Saturn
open Prometheus

open API.Prometheus
open API.DataScience

type CreatePayload =
    { id: int
      crimesPerCapta: float }

let private createController (ctx: HttpContext) =
    task {
        // TODO deal with problems when parsing the payload
        let! cnf = Controller.getJson<CreatePayload> ctx

        do createSummary.Observe cnf.id

        let histogram =
            createHistogram 
                ("create_request_id") 
                ("[POST] Histogram of the requests made.")
        do histogram.Observe cnf.id
        
        let predictionHistogram = 
            createHistogram 
                ("prediction_computation_histogram") 
                ("[POST] The histogram of the prediction computation.")

        let prediction =
            trackComputationHistogram 
                (predictionHistogram) 
                (getPrediction) 
                (cnf.crimesPerCapta)

        return! 
            (sprintf "Request OK\nId: %d\nCrimesPerCapta: %f\nPrice Prediction: %f" 
                (cnf.id) 
                (cnf.crimesPerCapta) 
                (prediction))
            |> Controller.text ctx
    }
    

let apiController = controller {
    create (createController)
}