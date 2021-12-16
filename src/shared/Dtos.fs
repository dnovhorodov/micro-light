module MicroLight.Dto

type DtoError = 
    | ValidationError of string
    | DeserializationException of exn

type ContactCreatedDto = 
    {
        FirstName: string
        MiddleName: string
        LastName: string
        EmailAddress: string
    }