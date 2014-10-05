open System
open Model
open EventStore.ClientAPI
open EventStore

let loadGame (esConn : IEventStoreConnection) =
    let events = readStream esConn ""

[<EntryPoint>]
let main argv = 

    printf "Welcome to D & D, please enter your name: "
    let playerName = Console.ReadLine()
    printf "Welcome %s" playerName

    let mutable running = false

    while (running) do
        let key = Console.ReadKey()
        running <- key != 'q'
        if running then
            let command = 
                match Console.ReadKey() with
                | 'l' -> Some (Move(Turn(Left)))
                | 'r' -> Some (Move(Turn(Right))
                | 'f' -> Some (Move(Forward))
                | 't' -> 
                    printfn "Trade not implemented yet"
                    None
             

    Console.ReadKey() |> ignore

    0 // return an integer exit code
