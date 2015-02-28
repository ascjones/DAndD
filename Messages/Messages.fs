namespace DAndD

module Messages = 

    type PlayerId = PlayerId of int

    type Direction = Left | Right
    
    type Command = 
        | JoinGame
        | Turn of Direction
        | MoveForwards

    type PlayerMessage = PlayerId * Command
