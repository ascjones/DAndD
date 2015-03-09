namespace DAndD

module Server = 

    open Akka.FSharp
    open DAndD.Messages
    open DAndD.Model

    [<EntryPoint>]
    let main argv = 
        
        let system = System.create DAndDServer <| Configuration.parse(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""

                    serializers {
                        json = ""Akka.Serialization.NewtonSoftJsonSerializer""
                    }

                    serialization-bindings { 
                      ""DAndD.Messages.GameMessage, DAndD"" = json
                    } 
                }

                remote {
                    helios.tcp {
                        port = 8080
                        hostname = localhost
                    }
                }
            }
        ")
        
        let gameId = GameId 1
        let grid = Levels.Level1 |> Seq.toList
        let game = system |> Game.create gameId

        game <! LoadGrid grid

        System.Console.ReadKey() |> ignore

        0 // return an integer exit code
