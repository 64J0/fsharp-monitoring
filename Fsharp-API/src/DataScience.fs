module API.DataScience

// From: https://fslab.org/content/tutorials/001_getting-started.html

open FSharp.Data
open Deedle
open FSharp.Stats
open Fitting.LinearRegression.OrdinaryLeastSquares

let load () =
    // Retrieve data using the FSharp.Data package
    let rawData = Http.RequestString @"https://raw.githubusercontent.com/dotnet/machinelearning/master/test/data/housing.txt"

    // And create a data frame object using the ReadCsvString method provided by Deedle.
    // Note: Of course you can directly provide the path to a local source.
    let df = Frame.ReadCsvString(rawData, hasHeaders=true, separators="\t")

    // Using the Print() method, we can use the Deedle pretty printer to have a look at the data set.
    df.Print()

    df

let predictPricesByRooms (description: string) data = 
    let pricesAll :Series<_,float> = 
        data
        |> Frame.getCol "MedianHomeValue"

    let roomsPerDwellingAll :Series<_,float> = 
        data
        |> Frame.getCol "RoomsPerDwelling"   

    let fit = 
        let tmpRooms, tmpPrices = 
            Series.zipInner roomsPerDwellingAll pricesAll    
            |> Series.sortBy fst
            |> Series.values
            |> Seq.toList
            |> List.unzip

        let coeffs = Linear.Univariable.coefficient (vector tmpRooms) (vector tmpPrices)
        Linear.Univariable.fit coeffs 
    fit 

let transform (df: Frame<int, string>) =
    let housesAtRiver = 
        df
        |> Frame.sliceCols ["RoomsPerDwelling";"MedianHomeValue";"CharlesRiver"]
        |> Frame.filterRowValues (fun s -> s.GetAs<bool>("CharlesRiver"))
        
    predictPricesByRooms "at river" housesAtRiver
    