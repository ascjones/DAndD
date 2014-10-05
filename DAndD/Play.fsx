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

let smallGrid = @"
# b
bgs
"

let buildGrid (gridAscii : string) = 
    let cells,_ = 
        gridAscii.Trim().ToCharArray()
        |> Seq.fold (fun (cells,nextCoord) ch ->

            if ch = '\010' then
                let coord = { nextCoord with X = 0; Y = nextCoord.Y + 1 }
                cells,coord
            else
                let cellState = 
                    match ch with
                    | '#' -> Blocked
                    | ' ' -> Empty
                    | 'b' -> ContainsItem(Bone)
                    | 'g' -> ContainsItem(GoldCoin)
                    | 's' -> ContainsItem(Scroll)
                    | x -> failwithf "Unsupported character %A" x
        
                let coord = { nextCoord with X = nextCoord.X + 1; Y = nextCoord.Y }
                let cell = { State = cellState; Coord = nextCoord }
                cell::cells,coord ) ([],{ X = 0; Y = 0 })
    cells

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
