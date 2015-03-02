namespace DAndD

module Model = 

    type GameId = GameId of int
    type PlayerId = PlayerId of int

    type Cell = Empty | Blocked | ContainsItem of Item
    and Item = Bone | GoldCoin | Scroll

    and Orientation = 
        | North 
        | East 
        | South 
        | West

    and Coord = { X : int; Y : int }

   

    





