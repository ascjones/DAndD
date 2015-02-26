namespace DAndD

module Model = 

    type GameId = GameId of string
    type PlayerId = PlayerId of string

    type Cell = Empty | Blocked | ContainsItem of Item
    and Item = Bone | GoldCoin | Scroll

    type PlayerState = 
        { Orientation : Orientation
          Coords : Coord 
          Items : Item list }
        static member New = 
            { Orientation = North; Coords = {X = 0; Y = 0}; Items = [] }

    and Orientation = 
        | North 
        | East 
        | South 
        | West

    and Coord = { X : int; Y : int }

    type GameState = 
        { Grid : Cell [,]
          PlayerLocations : Map<PlayerId, Coord> }

    type PlayerCommand =
        | JoinGame of GameId
        | Move of MoveCommand
        | Trade of TradeItemsCommand
    and MoveCommand = 
        | Forward 
        | Turn of Direction
    and Direction = 
        | Left 
        | Right
    and TradeItemsCommand = 
        { Sell1 : Item
          Sell2 : Item
          Sell3 : Item
          Buy : Item }

    type PlayerEvent = 
        | OrientationChanged of Orientation
        | Moved of Coord
        | ItemCollected of Item

    





