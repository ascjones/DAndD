module Model

open System

type Orientation = | North | East | South | West
type Coord = { X : int; Y : int }
type Player = { Id : string; Orientation : Orientation; Coords : Coord }

type CellState =
    | Empty 
    | Blocked
    | ContainsItem of Item
and Item = 
    | Bone
    | GoldCoin
    | Scroll

type Cell = { Coord : Coord; State : CellState }
type Grid = { Cells : Cell list }

type MoveCommand = 
    | Forward
    | Turn of Direction
and Direction = 
    | Left
    | Right

type TradeItemsCommand = 
    { Sell1 : Item
      Sell2 : Item
      Sell3 : Item
      Buy : Item }

type Command =
    | Move of MoveCommand
    | Trade of TradeItemsCommand

type Event =
    | PlayerOrientationChanged of playerId : string * orientation : Orientation
    | PlayerMoved of playerId : string * coord : Coord
    | ItemCollected of playerId : string * coord : Coord * item : Item

type Game =
    { Players : Player list
      Grid : Grid }

//type CommandHandler = Command -> Event list

let turn (orientation : Orientation) d = 
    match orientation with
    | North -> match d with Left -> West | Right -> East
    | South -> match d with Left -> East | Right -> West
    | East  -> match d with Left -> North | Right -> South
    | West  -> match d with Left -> South | Right -> North

let moveForward player = 
    match player.Orientation with
    | North -> { player.Coords with Y = player.Coords.Y + 1 }
    | South -> { player.Coords with Y = player.Coords.Y - 1 }
    | East -> { player.Coords with X = player.Coords.X + 1 }
    | West -> { player.Coords with X = player.Coords.X - 1 }

let handle game player command = 
    match command with
    | Move m ->
        match m with
        | Turn direction ->
            let newOrientation = turn player.Orientation direction
            PlayerOrientationChanged(player.Id, newOrientation)
        | Forward ->
            let newCoords = moveForward player
            PlayerMoved(player.Id, newCoords)
    | x -> failwithf "Unsupported command %A" x


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





