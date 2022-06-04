module API.DataScience

// From: https://fslab.org/content/tutorials/001_getting-started.html

open FSharp.Data
open Deedle
open FSharp.Stats
open Fitting.LinearRegression.OrdinaryLeastSquares

let private loadData () =
    // Retrieve data using the FSharp.Data package
    let rawData = Http.RequestString @"https://raw.githubusercontent.com/dotnet/machinelearning/master/test/data/housing.txt"

    // And create a data frame object using the ReadCsvString method provided by Deedle.
    // Note: Of course you can directly provide the path to a local source.
    Frame.ReadCsvString(rawData, hasHeaders=true, separators="\t")

let private transform (dataframe: Frame<int, string>) =
    // Filter the data to have only the MedianHomeValue, CrimesPerCapita and CharlesRiver
    // columns. Those are the only columns taken into account by our model.
    dataframe
    |> Frame.sliceCols ["MedianHomeValue";"CrimesPerCapita";"CharlesRiver"]
    |> Frame.filterRowValues (fun s -> s.GetAs<bool>("CharlesRiver"))

let private predictPriceByCrimesPerCapta (data: Frame<'a, string>) = 
    let pricesAll :Series<_,float> = 
        data
        |> Frame.getCol "MedianHomeValue"

    let crimesPerCaptaAll :Series<_,float> = 
        data
        |> Frame.getCol "CrimesPerCapita"   

    Series.zipInner crimesPerCaptaAll pricesAll    
    |> Series.sortBy fst
    |> Series.values
    |> Seq.toList
    |> List.unzip
    |> (fun (x1, x2) -> 
        Linear.Univariable.coefficient (vector x1) (vector x2))
    |> Linear.Univariable.fit

let getPredictionModel (crimesPerCapta: float) =
    loadData ()
    |> transform
    |> predictPriceByCrimesPerCapta <| (crimesPerCapta)
    