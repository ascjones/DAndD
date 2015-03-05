// ACTORS

#r @"..\packages\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll"
#r @"..\packages\FsPickler\lib\net45\FsPickler.dll"
#r @"..\packages\Akka\lib\net45\Akka.dll"
#r @"..\packages\Akka.FSharp\lib\net45\Akka.FSharp.dll"

#load "Model.fs"
#load "Messages.fs"
#load "Game.fs"
#load "Levels.fs"

open Akka.FSharp
open DAndD
open DAndD.Model
open DAndD.Messages

let system = System.create "DAndD" <| Configuration.load()

let gameId = GameId 1
let game = system |> Game.createGame gameId Levels.Level1

game <! (PlayerId 1, JoinGame)
game <! (PlayerId 1, PlayerCommand (Turn Right))
game <! (PlayerId 1, PlayerCommand (MoveForwards))
 