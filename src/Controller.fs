module API.Controller

open Saturn

let indexFn (ctx) =
    printfn "Called"
    
    "Index handler version 1" 
    |> Controller.text ctx

let apiController = controller {
    index (indexFn)
}