namespace DAndD

module Server = 

    open Akka.FSharp
    open DAndD.Messages

    let player msg = ()

    let game (mailbox : Actor<PlayerMessage>) (playerId,msg) =
        match msg with
        | JoinGame ->
            let playerActorId = match playerId with PlayerId id -> sprintf "player-%i" id
            printfn "Player %s requested to join the game" playerActorId
            //            spawn mailbox playerActorId (actorOf player)
        | x -> printfn "Command %A not yet implemented" x

    [<EntryPoint>]
    let main argv = 
        
        let system = System.create "DAndD" <| Configuration.load()

        // create a game
        let gameId = GameId 1
        let gameActorId = match gameId with GameId id -> sprintf "game-%i" id
        // spawn the game actor
        let game = spawn system gameActorId <| actorOf2 game

        game <! (PlayerId 1, JoinGame)

        System.Console.ReadKey() |> ignore

        0 // return an integer exit code
