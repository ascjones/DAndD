namespace DAndD

module Client = 

    open System
    open Akka.FSharp
    open DAndD.Contract

    let parseCommand (c : char) = 
        match Char.ToUpper(c) with
        | '<' -> Some (PlayerCommand <| Turn Left) // todo: make into GameMessage.PlayerRequest
        | '>' -> Some (PlayerCommand <| Turn Right)
        | '^' -> Some (PlayerCommand <| MoveForwards)
        | _ -> None

    let playerClient mailbox game msg = 
        match msg with
        | PlayerCommand cmd ->
            game <! msg
            game
        | CellResponse resp ->
            printfn "Response %A received" resp
            game

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
        InitSerialisation system

        let gameId = GameId 1
        let playerId = PlayerId 1

        let game = system |> select (sprintf "akka.tcp://DAndDServer@localhost:8080/user/%s" (gameIdStr gameId))
        let client = spawn system "cli" <| actorWithState playerClient game

        // JOIN GAME
        game <! PlayerRequest (playerId,JoinGame)

        while true do
            let key = Console.ReadKey().KeyChar
            let input = key |> parseCommand
            match input with
            | Some cmd ->
                client <! cmd
            | None -> printfn "Invalid key char %A" key
        
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
