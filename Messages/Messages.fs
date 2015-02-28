namespace DAndD

module Messages = 

    type GameId = GameId of int
    type PlayerId = PlayerId of int

    type Direction = Left | Right
    
    type Command = 
        | JoinGame
        | Turn of Direction
        | MoveForwards

    type PlayerMessage = PlayerId * Command
