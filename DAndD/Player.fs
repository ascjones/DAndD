namespace DAndD

module Player = 

    open Akka.FSharp
    open DAndD.Model

    let turn (orientation : Orientation) d = 
        let leftOrRight left right =
            match d with Left -> left | Right -> right
        match orientation with
        | North -> leftOrRight West East
        | South -> leftOrRight East West 
        | East  -> leftOrRight North South
        | West  -> leftOrRight South North

    let moveForward player = 
        let coords = player.Coords
        match player.Orientation with
        | North -> { coords with Y = coords.Y + 1 }
        | South -> { coords with Y = coords.Y - 1 }
        | East  -> { coords with X = coords.X + 1 }
        | West  -> { coords with X = coords.X - 1 }

    let handle cmd player system = 
        match cmd with
        | JoinGame gameId ->
            let gameAddress = Game.gameAddress "DAndD" gameId
            let game = select gameAddress system 
            game <! 
        | Move m ->
            match m with
            | Turn direction ->
                let newOrientation = turn player.Orientation direction
                [OrientationChanged newOrientation]
            | Forward ->
                let newCoords = moveForward player
                [Moved newCoords]
        | x -> failwithf "Unsupported command %A" x

    let apply evts player =
        evts
        |> List.fold (fun p evt -> 
            match evt with
            | OrientationChanged o -> { p with Orientation = o }
            | Moved coords -> { p with Coords = coords }
            | ItemCollected item -> { p with Items = item::player.Items }
        ) player
    
    let create id system =
        let playerId = match id with PlayerId s -> "player" + s
        spawn system playerId
        <| fun mailbox ->
            let rec loop player = actor {
                let! cmd = mailbox.Receive()
                let evts = player |> handle cmd
                let player' = player |> apply evts
                printfn "Player State %A" player'
                return! loop player' }
            loop <| PlayerState.New
