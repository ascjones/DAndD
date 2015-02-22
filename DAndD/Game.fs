namespace DAndD

module Game = 

    open Akka.FSharp
    open DAndD.Model

    let create gameId grid system = 
        let gameId' = match gameId with GameId id -> "game" + id
        spawn system gameId' 
        <| fun mailbox ->
            let rec loop () = actor {
                let! cmd = mailbox.Receive()
                match cmd with
                | Move m -> printfn "Moved %A" m
                | x -> printfn "%A" x
                return! loop () }
            loop ()

    
