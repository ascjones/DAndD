namespace DAndD

module Messages = 

    type GameId = GameId of int
    type PlayerId = PlayerId of int

    type GameCommand = 
        | JoinGame
        | QuitGame

    type PlayerCommand = 
        | Turn of Direction
        | MoveForwards
    and Direction = 
        | Left 
        | Right
    
    type Message = 
        | GameCommand of GameCommand
        | PlayerCommand of PlayerCommand

    type Request = GameId * PlayerId * Message
