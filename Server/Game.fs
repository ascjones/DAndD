namespace DAndD

module Game = 

    open Akka.Actor
    open Akka.FSharp
    open DAndD.Contract
    open DAndD.Helpers

    type Game = 
        { Id : GameId
          Grid : Map<Coord, ActorRef>
          Players : Map<PlayerId, ActorRef> }

    let private buildGrid game cells =
        cells
        |> List.map (fun (coord,cell) -> 
            let cellActor = Cell.createCell coord cell game
            coord,cellActor)
        |> Map.ofList

    let game mailbox state msg =
//        printfn "Received Coords %A" msg
//        state
        match msg with
        | LoadGrid cells ->
            printfn "Loading grid of %i cells" cells.Length
            let grid = buildGrid mailbox cells
            { state with Grid = grid }
        | PlayerRequest (playerId,req) ->
            match req with
            | JoinGame ->
                printfn "%A joined game" playerId
                let player = Player.createPlayer state.Id playerId mailbox
                { state with Players = state.Players |> Map.add playerId player }
            | PlayerCommand cmd -> 
                let player = state.Players |> Map.find playerId
                player <! PlayerMessage.PlayerCommand cmd
                state

    let create gameId system = 
        let id = gameId |> gameIdStr
        spawn system id <| actorWithState game { Id = gameId; Players = Map.empty; Grid = Map.empty }

    
