module API.Controller.Prediction

open System.Net
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

open Giraffe

open API.DataScience

type CreatePayload = { id: int; crimesPerCapta: float }

type SuccessResponse =
    { Message: string
      Id: int
      CrimesPerCapta: float
      PricePrediction: float }

/// POST /api/prediction endpoint
let createController () : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let serializer = ctx.GetJsonSerializer()
            let logger = ctx.GetLogger()

            try
                let! cnf = serializer.DeserializeAsync<CreatePayload> ctx.Request.Body

                let prediction = getPredictionModel cnf.crimesPerCapta

                let result: SuccessResponse =
                    { Message = "OK"
                      Id = cnf.id
                      CrimesPerCapta = cnf.crimesPerCapta
                      PricePrediction = prediction }

                return! (int HttpStatusCode.Created |> setStatusCode >=> json result) next ctx
            with exn ->
                logger.LogError $"An exception was raised when trying to calculate a prediction:\n{exn}"

                let result = {| Message = "The server was not able to handle this payload." |}

                return! (int HttpStatusCode.InternalServerError |> setStatusCode >=> json result) next ctx
        }
