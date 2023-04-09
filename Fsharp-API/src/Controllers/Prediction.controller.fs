module API.Controller.Prediction

open Microsoft.AspNetCore.Http
open Saturn

open API.DataScience

type CreatePayload = { id: int; crimesPerCapta: float }

/// POST /api/prediction endpoint
let private createController (ctx: HttpContext) =
    task {
        // TODO: deal with problems when parsing the payload.
        let! cnf = Controller.getJson<CreatePayload> ctx

        let prediction = getPredictionModel cnf.crimesPerCapta

        return!
            (sprintf
                "Request OK\nId: %d\nCrimesPerCapta: %f\nPrice Prediction: %f"
                (cnf.id)
                (cnf.crimesPerCapta)
                (prediction))
            |> Controller.text ctx
    }

let apiController = controller { create (createController) }
