namespace DAndD

module Model = 

    type GameId = GameId of int
    type PlayerId = PlayerId of int

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

   

    





