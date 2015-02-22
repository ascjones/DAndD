namespace DAndD

module Player = 

    open Akka.FSharp
    open DAndD.Model

    let turn (orientation : Orientation) d = 
        match orientation with
        | North -> match d with Left -> West | Right -> East
        | South -> match d with Left -> East | Right -> West
        | East  -> match d with Left -> North | Right -> South
        | West  -> match d with Left -> South | Right -> North

    let moveForward player = 
        let coords = player.Coords
        match player.Orientation with
        | North -> { coords with Y = coords.Y + 1 }
        | South -> { coords with Y = coords.Y - 1 }
        | East  -> { coords with X = coords.X + 1 }
        | West  -> { coords with X = coords.X - 1 }

    let handle cmd player = 
        match cmd with
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
                return! loop player' }
            loop <| PlayerState.New
