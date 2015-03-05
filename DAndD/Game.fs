namespace DAndD

module Game = 

    open Akka.Actor
    open Akka.FSharp
    open DAndD.Model
    open DAndD.Messages

    let actorWithState (fn : Actor<'Message> -> 'State -> 'Message -> 'State) (initialState : 'State) (mailbox : Actor<'Message>) : Cont<'Message, 'Returned> = 
        let rec loop state = 
            actor { 
                let! msg = mailbox.Receive()
                let newState = fn mailbox state msg
                return! loop newState
            }
        loop initialState

    type Player = 
        { Id : PlayerId
          Orientation : Orientation
          Coords : Coord 
          Items : Item list }
        static member New id = 
            { Id = id; Orientation = North; Coords = {X = 0; Y = 0}; Items = [] }

    type Game = 
        { Grid : Map<Coord, ActorRef>
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

    type CellMessage = PlayerMoveRequest of PlayerId
    type CellEvent = 
        | PlayerEntered of PlayerId
        | ItemCollected of Item
        | CellOccupied of PlayerId
        | CellBlocked

    let cell (mailbox : Actor<_>) state = function
        | PlayerMoveRequest playerId ->
            let player = mailbox.Sender()
            match state with
            | Occupied otherPlayerId -> 
                player <! CellOccupied otherPlayerId
                state
            | Blocked ->                
                player <! CellBlocked
                state
            | ContainsItem item ->      
                player <! ItemCollected item
                player <! PlayerEntered playerId
                Occupied playerId
            | Empty ->
                player <! PlayerEntered playerId
                Occupied playerId

    let player mailbox state = function
        | Turn direction -> 
            let newOrientation = state |> turn direction
            printfn "%A new orientation %A" state.Id newOrientation
            { state with Orientation = newOrientation }
        | MoveForwards ->
            let newCoords = moveForward state
            printfn "%A moved forwards %A" state.Id newCoords
            { state with Coords = newCoords }

    let createPlayer playerId game = 
        let idString = playerId |> playerIdStr
        spawn game idString <| actorWithState player (Player.New playerId)

    let game mailbox state msg =
        let playerId,cmd = msg
        match cmd with
        | JoinGame ->
            printfn "%A joined game" playerId
            let player = createPlayer playerId mailbox
            { state with Players = state.Players |> Map.add playerId player }
        | PlayerCommand cmd -> 
            let player = state.Players |> Map.find playerId
            player <! cmd
            state

    let createGame gameId grid system = 
        let gameId = gameId |> gameIdStr
        spawn system gameId <| actorWithState game { Players = Map.empty; Grid = Map.empty }

    
