namespace DAndD

module Game = 

    open Akka.FSharp
    open DAndD.Model
    open DAndD.Messages
    open DAndD.Player

    let private gameIdStr gameId = match gameId with GameId id -> sprintf "game-%i" id

    let gameAddress systemId gameId = sprintf "akka://%s/user/%s" systemId (gameId |> gameIdStr)

    let create gameId grid system = 
        let gameId' = gameId |> gameIdStr
        spawn system gameId'
        <| fun mailbox ->
            let rec loop () = actor {
                let! msg = mailbox.Receive()
                match msg with
                | PlayerEvent (playerId,evt) ->
                    match evt with
                    | OrientationChanged o -> printfn "Player %A has orientation %A" playerId o
                return! loop () }
            loop ()

    
