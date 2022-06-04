module API.Controller.Prediction

open Microsoft.AspNetCore.Http
open Saturn

open API.Prometheus
open API.DataScience

type CreatePayload =
    { id: int
      crimesPerCapta: float }

let private postSummaryLabels =
    [
        ("endpoint", "/api/prediction")
        ("verb", "post")
    ]

/// Prometheus Counter for the POST endpoint.
let private predictionCounter =
    let counterName = "post_prediction_counter"
    let counterDescription = "The quantity of times the POST prediction endpoint was called."
    createCounter (counterName) (counterDescription)

/// Prometheus Summary for the POST endpoint.
let private predictionSummary =
    let summaryName = "post_prediction_summary"
    let summaryDescription = "Prometheus summary metric for the POST /api/prediction endpoint."
    createSummary (summaryName) (summaryDescription) (postSummaryLabels)

let private predictionHistogram =
    let histogramName = "post_prediction_histogram"
    let histogramDescription = "Prometheus histogram metric for the POST /api/prediction endpoint."
    createHistogram (histogramName) (histogramDescription)

/// POST /api/prediction endpoint
let private createController (ctx: HttpContext) =
    task {
        // Increment the counter value everytime we get a new request.
        predictionCounter.Inc()
        // predictionSummary. // TODO: understand how to use this

        // TODO: deal with problems when parsing the payload.
        // TODO: add a counter to keep track of the failed requests.
        let! cnf = Controller.getJson<CreatePayload> ctx

        let prediction =
            trackComputationHistogram 
                (predictionHistogram) 
                (getPredictionModel) 
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