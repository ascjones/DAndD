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

                    serializers {
                        json = ""Akka.Serialization.NewtonSoftJsonSerializer""
                    }

                    serialization-bindings { 
                      ""DAndD.Messages.GameMessage, DAndD"" = json
                    } 
                }

                remote {
                    helios.tcp {
                        port = 8090
                        hostname = localhost
                    }
                }
            }
        ")

        let gameId = GameId 1
        let playerId = PlayerId 1

        let game = system |> select (sprintf "akka.tcp://DAndDServer@localhost:8080/user/%s" (gameIdStr gameId))

        game <! PlayerRequest (playerId,JoinGame)
        
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
