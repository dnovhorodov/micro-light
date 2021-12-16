namespace MicroLight

[<AutoOpen>]
module Constants =
    let [<Literal>] LoggerDateTimeFormat = "MM/dd/yy H:mm:ss.fff zz"
    let [<Literal>] TopicName = "test-topic"
    let [<Literal>] EmailRegexPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z"