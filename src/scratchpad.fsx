System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

// Pub/sub ZeroMQ tests
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: FSharp.Data, 4.2.2"
#r "nuget: FsConfig, 3.1.0"
#r "nuget: NetMQ, 4.0.1.6"
#load "./shared/Constants.fs"
#load "./shared/Domain.fs"
#load "./shared/MicroLightConfig.fs"
#load "./utils/Helpers.fs"
#load "./shared/Dtos.fs"
#load "./shared/Events.fs"
#load "./shared/DomainHelpers.fs"

open System
open FsConfig
open NetMQ
open NetMQ.Sockets
open MicroLight
open MicroLight.Domain
open MicroLight.Dto
open System.Text.RegularExpressions
open MicroLight.Helpers.Extensions

let config = {
    Server = ""
    Port = 5556us
    Topics = ["test-topic"]
    DelayInternalMs = 10
}

System.Environment.SetEnvironmentVariable("MICROLIGHT_SERVER", "pub")
System.Environment.SetEnvironmentVariable("MICROLIGHT_PORT", "123")
System.Environment.GetEnvironmentVariables()

let c = getConfig ()

// *** client ***
// let subscriber = new SubscriberSocket()
// subscriber.Options.ReceiveHighWatermark <- 1000
// subscriber.Connect("tcp://127.0.0.1:5556") // Using DNS name - tcp://server1:5556
// subscriber.Subscribe("content-access")
// let mutable message = NetMQMessage()
// let status = subscriber.TryReceiveMultipartMessage(&message, 2)
// if status then
//     let json = Encoding.UTF8.GetString(message.Last.ToByteArray())
//     let msg = json |> deserialize<ContentAccessRequest>
//     printfn $"Received %A{msg}"

// subscriber.Dispose()
// NetMQConfig.Cleanup()
// *** end client ***

// *** server ***
let publisher = new PublisherSocket()
publisher.Options.Linger <- TimeSpan.FromMilliseconds(1000.0)
publisher.Bind("tcp://127.0.0.1:5556")

// *** Send email sent event ***
let event : SendEmailDomainEvent = {
    Contact = {
        FirstName = "John"
        MiddleName = None
        LastName = "Dou"        
    }
    SendToAddress = EmailAddress "johndou@gmail.com"
}
let json = event |> SendEmailDto.jsonFromDomain
// publisher.SendMoreFrame("test-topic").SendFrame(json) // incorrect event
// publisher.SendMoreFrame("test-topic").SendMoreFrame("NotSupportedDomainEventType").SendFrame(json) // incorrect event 2
publisher.SendMoreFrame("test-topic").SendMoreFrame("Infrastructure").SendFrame(json) // correct event

publisher.Dispose()
NetMQConfig.Cleanup()
// *** end server ***