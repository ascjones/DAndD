namespace DAndD

[<AutoOpen>]
module Helpers =
    
    open Akka.Actor
    open Akka.FSharp
    open DAndD.Contract

    let actorWithState (fn : Actor<'Message> -> 'State -> 'Message -> 'State) (initialState : 'State) (mailbox : Actor<'Message>) : Cont<'Message, 'Returned> = 
        let rec loop state = 
            actor { 
                let! msg = mailbox.Receive()
                let newState = fn mailbox state msg
                return! loop newState
            }
        loop initialState

    let gameIdStr gameId = match gameId with GameId id -> sprintf "game-%i" id
    let gameAddress gameId = sprintf "akka://%s/user/%s" DAndDServer (gameId |> gameIdStr)

    let InitSerialisation (system : ActorSystem) = 
       let serializer = system.Serialization.GetSerializerById(9)
       system.Serialization.AddSerializationMap(typeof<GameMessage>, serializer)

