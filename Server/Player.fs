namespace DAndD

module Player = 

    open Akka.FSharp
    open DAndD.Contract
   
    type Player = 
        { Id : PlayerId
          GameId : GameId
          Orientation : Orientation
          Coords : Coord 
          Items : Item list }
        static member New gameId id = 
            { Id = id; GameId = gameId; Orientation = North; Coords = {X = 0; Y = 0}; Items = [] }

    let playerIdStr playerId = match playerId with PlayerId id -> sprintf "player-%i" id

    let private turn direction player = 
        let leftOrRight left right =
            match direction with Left -> left | Right -> right
        match player.Orientation with
        | North -> leftOrRight West East
        | South -> leftOrRight East West 
        | East  -> leftOrRight North South
        | West  -> leftOrRight South North

    let private facingCell coords orientation = 
        match orientation with
        | North -> { coords with Y = coords.Y + 1 }
        | South -> { coords with Y = coords.Y - 1 }
        | East  -> { coords with X = coords.X + 1 }
        | West  -> { coords with X = coords.X - 1 }

    let player mailbox state msg = 
        printfn "Player Actor %A handling %A" state msg
        let selectCell coords = 
            let address = sprintf "%s/%i-%i" (gameAddress state.GameId) coords.X coords.Y
            printfn "Selecting cell %s" address
            select address mailbox
        match msg with
        | PlayerMessage.PlayerCommand cmd ->
            match cmd with
            | Turn direction -> 
                let newOrientation = state |> turn direction
                let nextCellCoords = facingCell state.Coords newOrientation
                let nextCell = selectCell nextCellCoords
                nextCell <! View
                state
            | MoveForwards ->
                let newCoords = facingCell state.Coords state.Orientation
                let cell = selectCell newCoords
                cell <! Enter newCoords
                state   
        | PlayerMessage.CellResponse resp ->
            match resp with
            | EnterResponse er -> 
                match er with
                | EnterSuccess coord -> { state with Coords = coord }
                | ItemCollected item -> { state with Items = item::state.Items }
                | CellOccupied playerId -> state
                | CellBlocked -> state
            | ViewResponse cell ->
                printfn "Cell in front is %A" cell
                state

    let createPlayer gameId playerId game = 
        let idString = playerId |> playerIdStr
        spawn game idString <| actorWithState player (Player.New gameId playerId)
