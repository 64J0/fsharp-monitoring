open Saturn
open Giraffe

let app = application {
    use_router (text "Hello World from Saturn")
}

run app