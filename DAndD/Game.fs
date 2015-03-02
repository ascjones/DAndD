namespace DAndD

module Game = 

    open Akka.FSharp
    open DAndD.Model
    open DAndD.Messages
    open DAndD.Player

    type PlayerState = 
        { Orientation : Orientation
          Coords : Coord 
          Items : Item list }
        static member New = 
            { Orientation = North; Coords = {X = 0; Y = 0}; Items = [] }

    type GameState = 
        { Grid : Cell [,]
          Players : Map<PlayerId, PlayerState> }

    let private gameIdStr gameId = match gameId with GameId id -> sprintf "game-%i" id

    let gameAddress systemId gameId = sprintf "akka://%s/user/%s" systemId (gameId |> gameIdStr)

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

    let create gameId grid system = 
        let gameId' = gameId |> gameIdStr
        spawn system gameId'
        <| fun mailbox ->
            let rec loop game = actor {
                let! playerId,msg = mailbox.Receive()
                match msg with
                | JoinGame ->
                    let player = 
                | Turn direction ->
                    let player = game.Players |> Map.find playerId
                    let newOrientation = turn player.Orientation direction
                    let game' = { game with Players = game.Players |> Map.add playerId { player with Orientation = newOrientation } }
                    return! loop game'
                | MoveForwards ->
                    let player = game.Players |> Map.find playerId
                    let newCoords = moveForward player
                    let game' = { game with Players = game.Players |> Map.add playerId { player with Coords = newCoords } }
                    return! loop game'
                return! loop game }
            loop { Grid = grid; Players = Map.empty }

    
