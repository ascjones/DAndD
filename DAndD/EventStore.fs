namespace DAndD

[<AutoOpen>]
module AsyncExtensions =
    open System
    open System.Threading
    open System.Threading.Tasks
    type Microsoft.FSharp.Control.Async with
        static member Raise(ex) = Async.FromContinuations(fun (_,econt,_) -> econt ex)

        static member AwaitTask (t: Task) =
            let tcs = new TaskCompletionSource<unit>(TaskContinuationOptions.None)
            t.ContinueWith((fun _ -> 
                if t.IsFaulted then tcs.SetException t.Exception
                elif t.IsCanceled then tcs.SetCanceled()
                else tcs.SetResult(())), TaskContinuationOptions.ExecuteSynchronously) |> ignore
            async {
                try
                    do! Async.AwaitTask tcs.Task
                with
                | :? AggregateException as ex -> 
                    do! Async.Raise (ex.Flatten().InnerExceptions |> Seq.head) }

    open EventStore.ClientAPI

    type IEventStoreConnection with
        member this.AsyncConnect() = Async.AwaitTask(this.ConnectAsync())
        member this.AsyncReadStreamEventsForward stream start count resolveLinkTos =
            Async.AwaitTask(this.ReadStreamEventsForwardAsync(stream, start, count, resolveLinkTos))
        member this.AsyncAppendToStream stream expectedVersion events =
            Async.AwaitTask(this.AppendToStreamAsync(stream, expectedVersion, events))
        member this.AsyncSubscribeToAll(resolveLinkTos, eventAppeared) =
            Async.AwaitTask(this.SubscribeToAllAsync(resolveLinkTos, eventAppeared))

module EventStore =

    open System
    open System.Net
    open EventStore.ClientAPI
    open Serialization
    open Model

    let deserialize (event: ResolvedEvent) = deserializeUnion event.Event.EventType event.Event.Data
    let serialize event = 
        let typeName, data = serializeUnion event
        EventData(Guid.NewGuid(), typeName, true, data, null)

    let inline (!>) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> ^b) x) // exlicit operator to return implicit ConnectionSettings

    let create (ip: IPAddress) port =  
        
        let credentials = new SystemData.UserCredentials("admin", "changeit")
        let connSettings = 
            !> ConnectionSettings.Create()
                .SetDefaultUserCredentials(credentials) // .UseConsoleLogger().EnableVerboseLogging()
                .SetDefaultUserCredentials(new SystemData.UserCredentials("admin", "changeit"))
                .KeepRetrying()
                .KeepReconnecting()
        let conn = EventStoreConnection.Create(connSettings, new IPEndPoint(ip, port))
        conn.ConnectAsync() |> ignore
        conn

    let subscribe (projection: Event -> unit) (store: IEventStoreConnection) =       
        store.AsyncSubscribeToAll(true, (fun s e -> deserialize e |> Option.iter projection)) |> Async.RunSynchronously |> ignore
        store

    let readStream (store: IEventStoreConnection) streamId version count =
        async {
            let! slice = Async.AwaitTask <| store.ReadStreamEventsForwardAsync(streamId, version, count, true)

            let events = 
                slice.Events 
                |> Seq.choose deserialize
                |> Seq.toList
            
            let nextEventNumber = 
                if slice.IsEndOfStream 
                then None 
                else Some slice.NextEventNumber

            return events, slice.LastEventNumber, nextEventNumber }
        |> Async.RunSynchronously

    let appendToStream (store: IEventStoreConnection) streamId expectedVersion newEvents =
        async {
        let serializedEvents = Seq.map serialize newEvents
        do! Async.AwaitTask (store.AppendToStreamAsync(streamId, expectedVersion, serializedEvents)) |> Async.Ignore }
        |> Async.RunSynchronously