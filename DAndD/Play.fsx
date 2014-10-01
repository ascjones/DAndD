﻿#load "Model.fs"

open Model


let mutable player = { Id = "Andrew"; Coords = { X = 3; Y = 3 }; Orientation = Orientation.North }
let game = { Players = [player]; Grid = { Width = 10; Height = 10} }

let sendCommand cmd = 
    let event = handle game player cmd
    match event with
    | PlayerOrientationChanged p ->
        player <- p
        player
    | x -> failwithf "Unhandled Event %A" x

// turn left
sendCommand (Move(Turn(Left)))

// turn right
sendCommand (Move(Turn(Right)))
