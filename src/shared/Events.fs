namespace MicroLight.Events

open MicroLight.Domain
open MicroLight.Dto

type DomainEventType =
    | Business
    | Infrastructure

type ContactCreatedDomainEvent = 
    {
        Contact: Contact
    }
type DomainEvent = 
    | ContactCreated of Result<ContactCreatedDomainEvent, DtoError>
