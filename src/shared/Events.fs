namespace MicroLight.Events

open MicroLight.Domain
open MicroLight.Dto

(* 
    Define domain events here
*)

type DomainEvent = 
    | SendEmailEvent of Result<SendEmailDomainEvent, DtoError>
