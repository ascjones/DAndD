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
        Serialisation.Init system

        let gameId = GameId 1
        let playerId = PlayerId 1

        let game = system |> select (sprintf "akka.tcp://DAndDServer@localhost:8080/user/%s" (gameIdStr gameId))

        game <! PlayerRequest (playerId,JoinGame)
//        game <! { X = 1; Y = 2 }
        
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
