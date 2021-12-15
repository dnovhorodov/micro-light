namespace MicroLight

open FsConfig

[<Convention("MICROLIGHT")>]
type MicroLightConfig = {
    [<DefaultValue("127.0.0.1")>]
    Server: string
    [<DefaultValue("5556")>]
    Port: uint16
    [<ListSeparator(',')>]
    [<DefaultValue("test-topic")>]
    Topics: string list
    [<DefaultValue("50")>]
    DelayInternalMs: int
}

[<AutoOpen>]
module Parser = 
    let getConfig () = 
        match EnvConfig.Get<MicroLightConfig>() with
        | Ok config -> config
        | Error error -> 
            match error with
            | NotFound envVarName -> 
                failwithf "Environment variable %s not found" envVarName
            | BadValue (envVarName, value) ->
                failwithf "Environment variable %s has invalid value %s" envVarName value
            | NotSupported msg -> 
                failwith msg    