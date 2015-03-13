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

    let game (mailbox : Actor<_>) state msg =
        match msg with
        | LoadGrid cells ->
            printfn "Loading grid of %i cells" cells.Length
            let grid = buildGrid mailbox cells
            { state with Grid = grid }
        | PlayerRequest (playerId,req) ->
            match req with
            | JoinGame ->
                printfn "%A joined game" playerId
                let playerClient = mailbox.Sender ()
                let player = Player.createPlayer state.Id playerId mailbox playerClient
                { state with Players = state.Players |> Map.add playerId player }
            | PlayerRequest.PlayerCommand cmd -> 
                let player = state.Players |> Map.find playerId
                let clientRef = mailbox.Sender ()
                player <! PlayerMessage.PlayerCommand cmd
                state

    let create gameId system = 
        let id = gameId |> gameIdStr
        spawn system id <| actorWithState game { Id = gameId; Players = Map.empty; Grid = Map.empty }

    
