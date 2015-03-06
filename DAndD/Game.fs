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
          GameId : GameId
          Orientation : Orientation
          Coords : Coord 
          Items : Item list }
        static member New gameId id = 
            { Id = id; GameId = gameId; Orientation = North; Coords = {X = 0; Y = 0}; Items = [] }

    type Game = 
        { Id : GameId
          Grid : Map<Coord, ActorRef>
          Players : Map<PlayerId, ActorRef> }

    let private gameIdStr gameId = match gameId with GameId id -> sprintf "game-%i" id
    let private playerIdStr playerId = match playerId with PlayerId id -> sprintf "player-%i" id

    let gameAddress gameId = sprintf "akka://%s/user/%s" DAndD (gameId |> gameIdStr)

    let turn direction player = 
        let leftOrRight left right =
            match direction with Left -> left | Right -> right
        match player.Orientation with
        | North -> leftOrRight West East
        | South -> leftOrRight East West 
        | East  -> leftOrRight North South
        | West  -> leftOrRight South North

    let facingCell coords orientation = 
        let coords = coords
        match orientation with
        | North -> { coords with Y = coords.Y + 1 }
        | South -> { coords with Y = coords.Y - 1 }
        | East  -> { coords with X = coords.X + 1 }
        | West  -> { coords with X = coords.X - 1 }

    let cell (mailbox : Actor<_>) state (playerId,msg) = 
        let player = mailbox.Sender()
        match msg with
        | Enter coords ->
            match state with
            | Occupied otherPlayerId -> 
                player <! EnterResponse (CellOccupied otherPlayerId)
                state
            | Blocked ->                
                player <! EnterResponse CellBlocked
                state
            | ContainsItem item ->      
                player <! EnterResponse (ItemCollected item)
                player <! EnterResponse (EnterSuccess coords)
                Occupied playerId
            | Empty ->
                player <! EnterResponse (EnterSuccess coords)
                Occupied playerId
        | View -> 
            player <! ViewResponse state
            state

    let player mailbox state msg = 
        let selectCell coords = 
            let address = sprintf "%s/%i-%i" (gameAddress state.GameId) coords.X coords.Y
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

    let game mailbox state (playerId,cmd) =
        match cmd with
        | JoinGame ->
            printfn "%A joined game" playerId
            let player = createPlayer state.Id playerId mailbox
            { state with Players = state.Players |> Map.add playerId player }
        | PlayerCommand cmd -> 
            let player = state.Players |> Map.find playerId
            player <! PlayerMessage.PlayerCommand cmd
            state

    let createGame gameId grid system = 
        let id = gameId |> gameIdStr
        spawn system id <| actorWithState game { Id = gameId; Players = Map.empty; Grid = Map.empty }

    
