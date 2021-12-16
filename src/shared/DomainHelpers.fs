module MicroLight.DomainHelpers

open MicroLight.Events

[<AutoOpen>]
module rec Extensions =
    
    open System.Text
    open NetMQ
    open NetMQ.Sockets
    open Helpers.Extensions

    let logEvent event =
        match event with
        | ContactCreated sendEmail -> 
            match sendEmail with
            | Ok evnt -> printfn $"[{System.DateTime.Now.ToString(LoggerDateTimeFormat)}][EVENT::SendEmail]: RECEIVED %A{evnt}"
            | Error err -> printfn $"[{System.DateTime.Now.ToString(LoggerDateTimeFormat)}][EVENT::SendEmail]: ERROR {err}"
        event

    type NetMQMessage with
        static member TryGetDomainEvent(sub: SubscriberSocket) =
            let mutable message = NetMQMessage()
            let success = sub.TryReceiveMultipartMessage(&message, 3)
            if success && message.First.ConvertToString() = TopicName then
                let eventType = message.Item(1).ConvertToString() |> fromString<DomainEventType>
                let msg = message.Last.ToByteArray() |> Encoding.UTF8.GetString
                match (eventType, msg) with
                | (Some t, _) when t = Business -> 
                    msg 
                    |> ContactCreated.jsonToDomain
                    |> ContactCreated
                    |> (logEvent >> Some)
                | _, _ -> None
            else None
    
    module ContactCreated =

        open System
        open System.Text.RegularExpressions
        open Helpers.Json
        open MicroLight.Dto
        open MicroLight.Domain

        let jsonFromDomain event : string =
            event
            |> fromDomain
            |> serialize

        let fromDomain event : ContactCreatedDto =
            {
                FirstName = event.Contact.FirstName
                MiddleName = event.Contact.MiddleName |> Option.defaultValue String.Empty
                LastName = event.Contact.LastName
                EmailAddress = event.Contact.EmailAddress |> function | EmailAddress a -> a
            }

        let jsonToDomain json =
            json 
            |> deserialize<ContactCreatedDto> 
            |> Result.mapError DeserializationException
            |> Result.bind toDomain

        let toDomain dto =
            let validateContact () = 
                match (dto.FirstName, dto.MiddleName, dto.LastName) with
                | (first, _, _) when String.IsNullOrWhiteSpace(first) -> Error (ValidationError "First name is mandatory")
                | (_, _, last) when String.IsNullOrWhiteSpace(last) -> Error (ValidationError "Last name is mandatory")
                | _,_,_ -> Ok ()
            
            let validateEmailAddress () = 
                match dto.EmailAddress with
                | e when Regex.IsMatch(e, EmailRegexPattern, RegexOptions.IgnoreCase) -> Ok ()
                | _ -> Error (ValidationError "Incorrect email address")
            
            validateContact () 
            |> Result.bind validateEmailAddress
            |> Result.bind (fun _ ->
                Ok {
                    Contact = { 
                        FirstName = dto.FirstName
                        MiddleName = 
                            match dto.MiddleName with
                            | m when not (String.IsNullOrWhiteSpace(m)) -> Some m
                            | _ -> None
                        LastName = dto.LastName
                        EmailAddress = EmailAddress dto.EmailAddress
                    }
                })