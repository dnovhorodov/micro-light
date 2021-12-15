module MicroLight.DomainHelpers

open Dto
open MicroLight.Events
open Helpers.Extensions

[<AutoOpen>]
module Extensions =
    
    open System.Text
    open NetMQ
    open NetMQ.Sockets

    let logEvent (event: DomainEvent) =
        match event with
        | SendEmailEvent sendEmail -> 
            match sendEmail with
            | Ok evnt -> printfn $"[{System.DateTime.Now.ToString(LoggerDateTimeFormat)}][EVENT::SendEmail]: RECEIVED %A{evnt}"
            | Error err -> printfn $"[{System.DateTime.Now.ToString(LoggerDateTimeFormat)}][EVENT::SendEmail]: ERROR {err}"
        event

    type NetMQMessage with
        static member TryGetDomainEvent(sub: SubscriberSocket) =
            let mutable message = NetMQMessage()
            let success = sub.TryReceiveMultipartMessage(&message, 3)
            if success && message.First.ConvertToString() = ContentAccessTopic then
                let eventType = message.Item(1).ConvertToString() |> fromString<DomainEventType>
                let msg = message.Last.ToByteArray() |> Encoding.UTF8.GetString
                match (eventType, msg) with
                | (Some t, _) when t = Infrastructure -> 
                    msg 
                    |> SendEmailDto.jsonToDomain
                    |> SendEmailEvent
                    |> (logEvent >> Some)
                | _, _ -> None
            else None