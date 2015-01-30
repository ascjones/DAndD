module DAndD.Program

open System
open Model
open EventStore.ClientAPI
open EventStore
open Async

[<EntryPoint>]
let main argv = 

    let gameStreamId = "Game-1"
    let esConn = create "localhost" 1113

    printf "Welcome to D & D, please enter your name: "
    let playerName = Console.ReadLine()
    printf "Welcome %s" playerName

    let handle = 
        if EventStore.streamExists gameStramId then
            
        
    // if no existing game, create a new game
    // if there's an existing game, join that game

    let mutable running = false

    while (running) do
        let key = Console.ReadKey()
        running <- key != 'q'
        if running then
            let command = 
                match Console.ReadKey() with
                // todo replace these with arrow keys?
                | 'l' -> Some (Move(Turn(Left)))
                | 'r' -> Some (Move(Turn(Right))
                | 'f' -> Some (Move(Forward))
                | 't' -> 
                    printfn "Trade not implemented yet"
                    None
             

    Console.ReadKey() |> ignore

    0 // return an integer exit code
