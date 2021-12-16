namespace MicroLight.Domain

type EmailAddress = | EmailAddress of string

type Contact =
    {
        FirstName: string
        MiddleName: string option
        LastName: string
        EmailAddress: EmailAddress
    }