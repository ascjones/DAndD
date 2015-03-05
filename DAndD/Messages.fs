namespace DAndD

module Messages =

    open DAndD.Model

    type GameMessage = PlayerId * GameCommand

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


