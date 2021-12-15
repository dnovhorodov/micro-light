open System
open NetMQ
open NetMQ.Sockets
open MicroLight
open MicroLight.Domain
open MicroLight.Dto
open MicroLight.Helpers.Extensions

[<EntryPoint>]
let main argv =

    let env = Environment.GetEnvironmentVariable "DOTNET_ENVIRONMENT"
    let config = getConfig ()

    printfn "Pub server. Press Ctrl+C to exit."
    let mutable stopRequested = false
    let rand = Random(50)

    Console.CancelKeyPress.Add(fun arg -> 
        printfn "Exiting from Pub server..."
        stopRequested <- true
        arg.Cancel <- true)

    try
        use publisher = new PublisherSocket()
        publisher.Options.Linger <- TimeSpan.FromMilliseconds(1000.0)
        publisher.Bind($"tcp://*:{config.Port}")

        while not stopRequested do
            let randomizedTopic = rand.NextDouble();
            
            if randomizedTopic > 0.5 then
                let event : SendEmailDomainEvent = {
                    Contact = {
                        FirstName = "John"
                        MiddleName = None
                        LastName = "Dou"
                    }
                    SendToAddress = EmailAddress "johndou@gmail.com"
                } 
                printfn $"Sending event : {event}"

                let json = event |> SendEmailDto.jsonFromDomain
                let eventType = Infrastructure |> toString
                
                config.Topics |> List.iter (
                    fun topic -> publisher.SendMoreFrame(topic).SendMoreFrame(eventType).SendFrame(json))
            else
                let json = """{ "NonExisting":"Message" }"""
                printfn $"Sending some non-defined event..."
                config.Topics |> List.iter (
                    fun topic -> publisher.SendMoreFrame(topic).SendMoreFrame(Infrastructure |> toString).SendFrame(json))
            
            Threading.Thread.Sleep(1000);
    finally
        NetMQConfig.Cleanup()
    0