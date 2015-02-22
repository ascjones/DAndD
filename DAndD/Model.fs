namespace DAndD

module Model = 

    type Orientation = North | East | South | West
    type Coord = { X : int; Y : int }

    type GameId = GameId of string
    type PlayerId = PlayerId of string

    type PlayerState = 
        { Orientation : Orientation
          Coords : Coord 
          Items : Item list }
        static member New = 
            { Orientation = North; Coords = {X = 0; Y = 0}; Items = [] }

    and Cell = Empty | Blocked | ContainsItem of Item
    and Item = Bone | GoldCoin | Scroll

    type GameState = 
        { Grid : Cell [,]
          Players : PlayerState list }

    type MoveCommand = Forward | Turn of Direction
    and Direction = | Left | Right

    type TradeItemsCommand = 
        { Sell1 : Item
          Sell2 : Item
          Sell3 : Item
          Buy : Item }

    type Command =
        | JoinGame of GameId
        | Move of MoveCommand
        | Trade of TradeItemsCommand

    type PlayerCommand =
        { PlayerId : string
          Command : Command }

    type GameCommand = 
        | NewGame of string

    type PlayerEvent = 
        | OrientationChanged of Orientation
        | Moved of Coord
        | ItemCollected of Item

    type PlayerEventMessage = PlayerId * PlayerEvent

    





