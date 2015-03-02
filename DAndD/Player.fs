namespace DAndD

module Player = 

    open Akka.FSharp
    open DAndD.Model
    open DAndD.Messages
    
//    let joinGame system playerId game =
//        let playerAddress = match playerId with PlayerId i -> sprintf "player-%i" i
//        spawn system playerAddress
//        <| fun mailbox ->
//            let rec loop player = actor {
//                let! msg = mailbox.Receive()
//                match msg with
//                | Command cmd -> 
//                    match cmd with
//                    | Turn direction ->
//                        let orientation = turn player.Orientation direction
//                        let evt = OrientationChanged orientation
//                        game <! PlayerEvent (playerId, evt)
//                        let player' = apply [evt] player
//                        return! loop player'
//                    | MoveForwards ->
//                        game <! 
//                | Event evt -> return! loop player
//                return! loop player }
//            loop <| PlayerState.New
