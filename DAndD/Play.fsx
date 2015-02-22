// ACTORS

#r @"..\packages\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll"
#r @"..\packages\FsPickler\lib\net45\FsPickler.dll"
#r @"..\packages\Akka\lib\net45\Akka.dll"
#r @"..\packages\Akka.FSharp\lib\net45\Akka.FSharp.dll"

#load "Model.fs"
#load "Game.fs"
#load "Player.fs"
#load "Levels.fs"

open Akka.FSharp
open DAndD
open DAndD.Model

let system = System.create "DAndD" <| Configuration.load()

let game = system |> Game.create (GameId "1") Levels.Level1

let player1 = system |> Player.create (PlayerId "Andrew")

player1 <! Move Forward