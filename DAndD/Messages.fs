namespace DAndD.Messages

open DAndD.Model

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


