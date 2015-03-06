namespace DAndD

module Model = 

    [<Literal>]
    let DAndD = "DAndD"

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

    and Coord = { X : int; Y : int }

   

    





