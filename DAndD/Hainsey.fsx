let x = 1

let add x y = 
    x + y

let add2 = add 2

let n = [1;2;3]

let n2 = 
    n 
    |> List.map (fun x -> x * 2)
    |> List.filter (fun x -> x % 2 = 0)

type MoveCommand = 
    | Forward
    | Turn of Direction
and Direction = 
    | Left
    | Right


let handle command = 
    match command with
    | Forward -> printfn "Forward"
    | Turn d -> 
        match d with 
        | Left -> printfn "Left"
        | Right -> printfn "Right"

handle (Forward)

type Person = { Id : int; Name : string }

let p1 = {Id = 1; Name = "Hainsey"}
let p2 = {Id = 1; Name = "Hainsey"}

p1 = p2

