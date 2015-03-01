namespace DAndD

module Server = 

    open Akka.Actor
    open Akka.FSharp
    open DAndD.Messages

    type GameState = 
        { Grid : Cell [,] 
          Players : Map<PlayerId,Player> }
    and Cell = Empty | Blocked | ContainsItem of Item
    and Item = Bone | GoldCoin | Scroll
    and Player = 
        { Actor : ActorRef 
          Location : Coords }
    and Coords = { X : int; Y : int }

    let player msg = ()

    [<EntryPoint>]
    let main argv = 
        
        let system = System.create "DAndD" <| Configuration.load()

        // create a game
        let gameId = GameId 1
        let gameActorId = match gameId with GameId id -> sprintf "game-%i" id
        // spawn the game actor
        let game = 
            spawn system gameActorId 
            <| fun mailbox -> 
                let rec loop state = actor {
                    let! (playerId,msg) = mailbox.Receive()
                    match msg with
                    | GameCommand gc ->
                        match gc with
                        | JoinGame ->
                            let playerActorId = match playerId with PlayerId id -> sprintf "player-%i" id
                            printfn "Player %s requested to join the game" playerActorId
                            let playerActor = spawn mailbox playerActorId (actorOf player)
                            let player = { Actor = playerActor; Location = { X = 0; Y = 0 } }
                            return! loop { state with Players = state.Players |> Map.add playerId player }
                        | x -> 
                             printfn "Command %A not implemented" x
                             return! loop state
                    | PlayerCommand pc -> 
                        match pc with
                        | Turn direction ->
                            printfn "Player turn %A requested" direction
                            return! loop state
                        | MoveForwards ->
                            printfn "Player move forwards requested"
                            return! loop state
                }
                loop { Grid = Array2D.zeroCreate 10 10; Players = Map.empty } // todo init game correctly
                

        game <! (PlayerId 1, GameCommand JoinGame)
        game <! (PlayerId 1, PlayerCommand <| Turn Left)
        game <! (PlayerId 1, PlayerCommand <| MoveForwards)

        System.Console.ReadKey() |> ignore

        0 // return an integer exit code
