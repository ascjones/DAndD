namespace DAndD

module Serialisation =

     open Akka.Actor
     open DAndD.Messages
    
     let Init (system : ActorSystem) = 
        let serializer = system.Serialization.GetSerializerById(9)
        system.Serialization.AddSerializationMap(typeof<GameMessage>, serializer)

