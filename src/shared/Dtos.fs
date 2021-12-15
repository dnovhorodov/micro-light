module MicroLight.Dto

open System
open System.Text.RegularExpressions
open Helpers.Json
open MicroLight.Domain

(*
    DTOs and functions for converting to and from domain models
*)

type DomainEventType =
    | Business
    | Infrastructure

type DtoError = 
    | ValidationError of string
    | DeserializationException of exn

type SendEmailDto = 
    {
        FirstName: string
        MiddleName: string
        LastName: string
        SendToAddress: string
    }

module rec SendEmailDto =

    let jsonFromDomain (event: Domain.SendEmailDomainEvent) : string =
        event
        |> fromDomain
        |> serialize

    let fromDomain (event: Domain.SendEmailDomainEvent) =
        {
            FirstName = event.Contact.FirstName
            MiddleName = event.Contact.MiddleName |> Option.defaultValue String.Empty
            LastName = event.Contact.LastName
            SendToAddress = event.SendToAddress |> function | EmailAddress a -> a
        }

    let jsonToDomain (json: string) : Result<Domain.SendEmailDomainEvent, DtoError> =
        json 
        |> deserialize<SendEmailDto> 
        |> Result.mapError DeserializationException
        |> Result.bind toDomain

    let toDomain (dto: SendEmailDto) : Result<Domain.SendEmailDomainEvent, DtoError> =
        let validateContact () = 
            match (dto.FirstName, dto.MiddleName, dto.LastName) with
            | (first, _, _) when String.IsNullOrWhiteSpace(first) -> Error (ValidationError "First name is mandatory")
            | (_, _, last) when String.IsNullOrWhiteSpace(last) -> Error (ValidationError "Last name is mandatory")
            | _,_,_ -> Ok ()
        
        let validateEmailAddress () = 
            match dto.SendToAddress with
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
                }
                SendToAddress = EmailAddress dto.SendToAddress
            })
