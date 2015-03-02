namespace DAndD

module Messages =

    open DAndD.Model

    type PlayerMessage = PlayerId * PlayerCommand

    and PlayerCommand =
        | JoinGame
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


