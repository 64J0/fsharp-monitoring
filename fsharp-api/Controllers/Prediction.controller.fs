module API.Controller.Prediction

open System.Net
open Microsoft.AspNetCore.Http

open Giraffe

open API.DataScience

type CreatePayload = { id: int; crimesPerCapta: float }

/// POST /api/prediction endpoint
let createController () : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            // TODO: deal with problems when parsing the payload.
            let serializer = ctx.GetJsonSerializer()
            let! cnf = serializer.DeserializeAsync<CreatePayload> ctx.Request.Body

            let prediction = getPredictionModel cnf.crimesPerCapta

            // TODO: create a type for this return structure
            let result =
                {| Message = "OK"
                   Id = cnf.id
                   CrimesPerCapta = cnf.crimesPerCapta
                   PricePrediction = prediction |}

            return! (int HttpStatusCode.OK |> setStatusCode >=> json result) next ctx
        }
