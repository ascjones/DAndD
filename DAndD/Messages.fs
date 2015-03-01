namespace DAndD

module Messages =

    open DAndD.Model

    //
    // Player Messages
    //

    type PlayerMessage =
        | Command of PlayerCommand
        | Event of PlayerEvent

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

    and PlayerEvent = 
        | OrientationChanged of Orientation
        | Moved of Coord
        | ItemCollected of Item

    //
    // Game Messages
    //

    type GameMessage = 
        | PlayerEvent of PlayerId * PlayerEvent



