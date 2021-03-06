﻿namespace DAndD

module Contract = 

    [<Literal>]
    let DAndDServer = "DAndDServer"

    [<Literal>]
    let DAndDClient = "DAndDClient"

    type GameId = GameId of int
    type PlayerId = PlayerId of int

    type Cell = 
        | Empty 
        | Blocked 
        | ContainsItem of Item
        | Occupied of PlayerId
    and Item = Bone | GoldCoin | Scroll

    and Orientation = 
        | North 
        | East 
        | South 
        | West

    [<StructuredFormatDisplay("({AsString})")>]
    type Coord = 
        { X : int; Y : int }
        member this.AsString = sprintf "X=%i, Y=%i" this.X this.Y

    type GameMessage = 
        | LoadGrid of (Coord * Cell) list
        | PlayerRequest of PlayerId * PlayerRequest

    and PlayerMessage =
        | PlayerCommand of PlayerCommand
        | CellResponse of CellResponse

    and PlayerRequest = 
        | JoinGame
        | PlayerCommand of PlayerCommand

    and PlayerCommand =
        | Turn of Direction
        | MoveForwards
        | Trade of TradeItemsCommand
              
    and Direction = 
        | Left 
        | Right

    and TradeItemsCommand = 
        { Sell1 : Item
          Sell2 : Item
          Sell3 : Item
          Buy : Item }

    and CellRequest = 
        | Enter of Coord
        | View

    and CellResponse =
        | EnterResponse of EnterResponse
        | ViewResponse of Cell

    and EnterResponse = 
        | EnterSuccess of Coord
        | ItemCollected of Item
        | CellOccupied of PlayerId
        | CellBlocked

    and ClientMessage = 
        | PlayerCommand of PlayerCommand
        | CellResponse of CellResponse


