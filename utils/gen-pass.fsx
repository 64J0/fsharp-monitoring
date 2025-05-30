#r "nuget: BCrypt.Net-Next, 4.0.3"

// Use this script to generate the bcrypt hashed password.
// This password is later used in the Prometheus configuration to set the
// authentication credentials.
//
// How to use it?
// dotnet fsi gen-pass.fsx

printfn "Enter a password to hash: "
let password = System.Console.ReadLine()
let hashedPassword = BCrypt.Net.BCrypt.HashPassword(password)
printfn "Hashed password: %s" hashedPassword
