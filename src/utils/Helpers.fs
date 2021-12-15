module MicroLight.Helpers

[<AutoOpen>]
module Json =

    open Newtonsoft.Json

    let serialize obj = JsonConvert.SerializeObject obj
    let deserialize<'a> str =
        try
            JsonConvert.DeserializeObject<'a> str |> Ok
        with
            // catch all exceptions and convert to Result
            | ex -> Error ex

[<AutoOpen>]
module Extensions =

    open Microsoft.FSharp.Reflection

    // Extensions for DUs
    let toString (x:'a) = 
        match FSharpValue.GetUnionFields(x, typeof<'a>) with
        | case, _ -> case.Name

    let fromString<'a> (s:string) =
        match FSharpType.GetUnionCases typeof<'a> 
            |> Array.filter (fun case -> case.Name = s) with
            |[|case|] -> Some(FSharpValue.MakeUnion(case,[||]) :?> 'a)
            |_ -> None    