namespace DAndD

module Cell = 

    open Akka.FSharp
    open DAndD.Contract
    open DAndD.Helpers

    let coordsAsStr coord = sprintf "%i-%i" coord.X coord.Y

    let private cellActor (mailbox : Actor<_>) state (playerId,msg) = 
        printfn "Cell actor %A handling %A from %A" state playerId msg 
        let player = mailbox.Sender()
        match msg with
        | Enter coords ->
            match state with
            | Occupied otherPlayerId -> 
                player <! PlayerMessage.CellResponse (EnterResponse (CellOccupied otherPlayerId))
                state
            | Blocked ->                
                player <! PlayerMessage.CellResponse (EnterResponse CellBlocked)
                state
            | ContainsItem item ->      
                player <! PlayerMessage.CellResponse (EnterResponse (ItemCollected item))
                player <! PlayerMessage.CellResponse (EnterResponse (EnterSuccess coords))
                Occupied playerId
            | Empty ->
                player <! PlayerMessage.CellResponse (EnterResponse (EnterSuccess coords))
                Occupied playerId
        | View -> 
            player <! PlayerMessage.CellResponse (ViewResponse state)
            state

    let createCell coord state game = 
        let cellActorId = coord |> coordsAsStr
        spawn game cellActorId <| actorWithState cellActor state

