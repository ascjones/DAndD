namespace DAndD

module Client = 

    open Akka.FSharp
    open DAndD.Messages
    open DAndD.Model

    [<EntryPoint>]
    let main argv = 

        let system = System.create DAndDClient <| Configuration.parse(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }

                remote {
                    helios.tcp {
                        port = 8090
                        hostname = localhost
                    }
                }
            }
        ")
        
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
