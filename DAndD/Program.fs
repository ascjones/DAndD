module DAndD.Program

open System
open Model
open Akka
open Akka.FSharp

[<EntryPoint>]
let main argv = 

    let gameId = "1"

    let system = System.create "DAndD" <| Configuration.load()

//    let game = system |> Actors.createGame "1"
//    game <! (Move Forward)

    Console.ReadKey() |> ignore

//    printf "Welcome to D & D, please enter your name: "
//    let playerName = Console.ReadLine()
//    printf "Welcome %s" playerName
        
    // if no existing game, create a new game
    // if there's an existing game, join that game

//    let mutable running = false
//
//    while (running) do
//        let key = Console.ReadKey()
//        running <- key != 'q'
//        if running then
//            let command = 
//                match Console.ReadKey() with
//                // todo replace these with arrow keys?
//                | 'l' -> Some (Move(Turn(Left)))
//                | 'r' -> Some (Move(Turn(Right))
//                | 'f' -> Some (Move(Forward))
//                | 't' -> 
//                    printfn "Trade not implemented yet"
//                    None
//             
//
//    Console.ReadKey() |> ignore

    0 // return an integer exit code
