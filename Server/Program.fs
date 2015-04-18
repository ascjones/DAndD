namespace DAndD

module Server = 

    open Akka.FSharp
    open DAndD.Contract

    [<EntryPoint>]
    let main argv = 
        
        let system = System.create DAndDServer <| Configuration.parse(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }

                remote {
                    helios.tcp {
                        port = 8080
                        hostname = localhost
                    }
                }
            }
        ")
        InitSerialisation system
        
        let gameId = GameId 1
        let grid = Levels.Level1 |> Seq.toList
        let game = system |> Game.create gameId

        game <! LoadGrid grid

        System.Console.ReadKey() |> ignore

        0
