let grid = @"
########
#      #    
#####  #
#      #
#   #  #
#####  #
#      #
########
"

//let mutable player = { Id = "Andrew"; Coords = { X = 3; Y = 3 }; Orientation = Orientation.North }
//let game = { Players = [player]; Grid = { Width = 10; Height = 10} }
//
//let sendCommand cmd = 
//    let event = handle game player cmd
//    match event with
////    | PlayerOrientationChanged p ->
////        player <- p
////        player
//    | PlayerMoved p ->
//        player <- p
//    | x -> failwithf "Unhandled Event %A" x
//
//// turn left
//sendCommand (Move(Turn(Left)))
//
//// turn right
//sendCommand (Move(Turn(Right)))
//
//// move forward
//sendCommand (Move(Forward))

// ACTORS

#r @"..\packages\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll"
#r @"..\packages\FsPickler\lib\net45\FsPickler.dll"
#r @"..\packages\Akka\lib\net45\Akka.dll"
#r @"..\packages\Akka.FSharp\lib\net45\Akka.FSharp.dll"

#load "Model.fs"
#load "Game.fs"

open Akka.FSharp
open DAndD
open DAndD.Model
open DAndD.Game

let system = System.create "DAndD" <| Configuration.load()

let game = system |> Game.create "1"
let move = Foo
game <! Bar