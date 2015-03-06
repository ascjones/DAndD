namespace DAndD

module Messages =

    open DAndD.Model

    type GameMessage = PlayerId * GameCommand

    and PlayerMessage =
        | PlayerCommand of PlayerCommand
        | CellResponse of CellResponse

    and GameCommand = 
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


