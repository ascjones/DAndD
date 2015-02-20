namespace DAndD

module Game = 

    open Akka.FSharp
    open DAndD.Model

    type Message = 
        | Foo
        | Bar

    let create gameId system = 
        spawn system ("game" + gameId) 
        <| fun mailbox ->
            let rec loop () = actor {
                let! cmd = mailbox.Receive()
                match cmd with
                | Foo -> printfn "%A" cmd
                | Bar -> printfn "%A" cmd
                return! loop () }
            loop ()

