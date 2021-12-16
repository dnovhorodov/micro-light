open System
open NetMQ
open NetMQ.Sockets
open MicroLight
open MicroLight.DomainHelpers

[<EntryPoint>]
let main argv =
    
    let env = Environment.GetEnvironmentVariable "DOTNET_ENVIRONMENT"
    let config = getConfig ()

    printfn "Starting Sub client..."

    let runContinuously sub = async {
        let rec fetch () = async {
            (* Define worflows here based on event type *)
            match NetMQMessage.TryGetDomainEvent(sub) with
            | Some event -> printfn $"%A{event}"
            | None -> ()

            do! Async.Sleep config.DelayInternalMs
            do! fetch ()
        }
        do! fetch ()
    }

    let listen () = async {
        use subscriber = new SubscriberSocket()
        subscriber.Options.ReceiveHighWatermark <- 1000
        subscriber.Options.HeartbeatInterval <- TimeSpan(0, 0, 0, 5)  
        subscriber.Options.HeartbeatTimeout <- TimeSpan(0, 0, 0, 30)
        subscriber.Options.HeartbeatTtl <- TimeSpan(0, 0, 0, 30)
        subscriber.Connect($"tcp://{config.Server}:{config.Port}")
        printfn $"Running environment: {env}"
        printfn $"Connected to {config.Server} on port {config.Port}"
        config.Topics |> List.iter (fun topic -> subscriber.Subscribe(topic))
        printfn $"""Start listening for events on the following topics: [{config.Topics |> String.concat ", "}]..."""
        printfn "(Press Ctrl+C to exit)"

        do! runContinuously subscriber
    }
    
    try
        try
            let cts = new Threading.CancellationTokenSource()
            Console.CancelKeyPress.Add(fun arg -> arg.Cancel <- true; cts.Cancel())
            Async.RunSynchronously(listen(), 1000, cts.Token)
            0
        with
            | Failure msg -> eprintfn $"General Failure: {msg}"; -1
            | :? OperationCanceledException -> eprintfn "Exiting from Sub client..."; 0
    finally
        NetMQConfig.Cleanup()