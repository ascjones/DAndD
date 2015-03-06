namespace DAndD

module Cell = 

    open Akka.FSharp
    open DAndD.Model
    open DAndD.Messages
    open DAndD.Helpers

    let coordsAsStr coord = sprintf "%i-%i" coord.X coord.Y

    let private cellActor (mailbox : Actor<_>) state (playerId,msg) = 
        printfn "Cell actor %A handling %A from %A" state playerId msg 
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

    let createCell coord state game = 
        let cellActorId = coord |> coordsAsStr
        spawn game cellActorId <| actorWithState cellActor state

