open System
open System.Net.Http
open NBomber.FSharp
open NBomber.Http
open NBomber.Http.FSharp
open NBomber.Contracts
open NBomber.Plugins.Network.Ping

module SimpleHttp =
    let run () =
        use httpClient = new HttpClient()

        let scenario =
            Scenario.create (
                "simple_http_scenario",
                fun context ->
                    task {
                        let request =
                            Http.createRequest "GET" $"http://localhost:8085/ping/foo-{System.DateTime.UtcNow.Millisecond}"
                            |> Http.withHeader "Accept" "application/json"

                        let! response = Http.send httpClient request

                        return response
                    }
            )
            |> Scenario.withoutWarmUp
            |> Scenario.withLoadSimulations
                [ RampingInject(rate = 50, interval = TimeSpan.FromSeconds(1.0), during = TimeSpan.FromMinutes(1.0))
                  Inject(rate = 50, interval = TimeSpan.FromSeconds(1.0), during = TimeSpan.FromMinutes(1.0))
                  RampingInject(rate = 0, interval = TimeSpan.FromSeconds(1.0), during = TimeSpan.FromMinutes(1.0)) ]

        NBomberRunner.registerScenario scenario
        |> NBomberRunner.withWorkerPlugins
            [ new PingPlugin(PingPluginConfig.createDefault ("localhost:8085/health"))
              new HttpMetricsPlugin(seq { HttpVersion.Version1 }) ]
        |> NBomberRunner.run

[<EntryPoint>]
let main (args: string array) =
    try
        SimpleHttp.run () |> ignore
        0 // Exit code 0 indicates success
    with ex ->
        printfn "An error occurred: %s" ex.Message
        1 // Exit code 1 indicates failure
