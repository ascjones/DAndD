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
          Orientation : Orientation
          Location : Coords }
    and Orientation = North | South | East | West
    and Coords = { X : int; Y : int }

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
                            let player = { Actor = mailbox.Sender (); Location = { X = 0; Y = 0 }; Orientation = East }
                            return! loop { state with Players = state.Players |> Map.add playerId player }
                        | x -> 
                             printfn "Command %A not implemented" x
                             return! loop state
                    | PlayerCommand pc -> 
//                        let player = state.Players |> Map.find playerId
//                        player.Actor <! 
                        return! loop state
                }
                loop { Grid = Array2D.zeroCreate 10 10; Players = Map.empty } // todo init game correctly
                

        game <! (PlayerId 1, GameCommand JoinGame)
        game <! (PlayerId 1, PlayerCommand <| Turn Left)
        game <! (PlayerId 1, PlayerCommand <| MoveForwards)

        System.Console.ReadKey() |> ignore

        0 // return an integer exit code
