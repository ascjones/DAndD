namespace DAndD

module Game = 

    open Akka.Actor
    open Akka.FSharp
    open DAndD.Model
    open DAndD.Messages

    type PlayerState = 
        { Orientation : Orientation
          Coords : Coord 
          Items : Item list }
        static member New = 
            { Orientation = North; Coords = {X = 0; Y = 0}; Items = [] }

    type GameState = 
        { Grid : Cell [,]
          Players : Map<PlayerId, ActorRef> }

    let private gameIdStr gameId = match gameId with GameId id -> sprintf "game-%i" id
    let private playerIdStr playerId = match playerId with PlayerId id -> sprintf "player-%i" id

    let gameAddress systemId gameId = sprintf "akka://%s/user/%s" systemId (gameId |> gameIdStr)

    let turn direction player = 
        let leftOrRight left right =
            match direction with Left -> left | Right -> right
        match player.Orientation with
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

    let createPlayer playerId game = 
        let playerId = playerId |> playerIdStr
        spawn game playerId
        <| fun mailbox ->
            let rec loop player = actor {
                let! msg = mailbox.Receive()
                match msg with
                | Turn direction -> 
                    let newOrientation = player |> turn direction
                    printfn "%A new orientation %A" playerId newOrientation
                    return! loop { player with Orientation = newOrientation }
                | MoveForwards ->
                    let newCoords = moveForward player
                    printfn "%A moved forwards %A" playerId newCoords
                    return! loop { player with Coords = newCoords } }
            loop PlayerState.New

    let create gameId grid system = 
        let gameId = gameId |> gameIdStr
        spawn system gameId
        <| fun mailbox ->
            let rec loop game = actor {
                let! playerId,msg = mailbox.Receive()
                match msg with
                | JoinGame ->
                    printfn "%A joined game" playerId
                    let player = createPlayer playerId mailbox
                    let game' = { game with Players = game.Players |> Map.add playerId player }
                    return! loop game'
                | PlayerCommand cmd -> 
                    let player = game.Players |> Map.find playerId
                    player <! cmd
                    return! loop game
                return! loop game }
            loop { Grid = grid; Players = Map.empty }

    
