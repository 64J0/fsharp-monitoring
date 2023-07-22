module API.Controller.Prediction

open Microsoft.AspNetCore.Http
open Giraffe
open Saturn

open API.DataScience

type CreatePayload = { id: int; crimesPerCapta: float }

/// POST /api/prediction endpoint
let createController () : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            // TODO: deal with problems when parsing the payload.
            let! cnf = Controller.getJson<CreatePayload> ctx

            let prediction = getPredictionModel cnf.crimesPerCapta

            let result =
                (sprintf
                    "Request OK\nId: %d\nCrimesPerCapta: %f\nPrice Prediction: %f"
                    (cnf.id)
                    (cnf.crimesPerCapta)
                    (prediction))

            return! (setStatusCode 200 >=> text result) next ctx
        }
