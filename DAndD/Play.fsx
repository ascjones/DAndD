#load "Model.fs"

open Model
open System

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

let mutable player = { Id = "Andrew"; Coords = { X = 3; Y = 3 }; Orientation = Orientation.North }
let game = { Players = [player]; Grid = { Width = 10; Height = 10} }

let sendCommand cmd = 
    let event = handle game player cmd
    match event with
//    | PlayerOrientationChanged p ->
//        player <- p
//        player
    | PlayerMoved p ->
        player <- p
    | x -> failwithf "Unhandled Event %A" x

// turn left
sendCommand (Move(Turn(Left)))

// turn right
sendCommand (Move(Turn(Right)))

// move forward
sendCommand (Move(Forward))
