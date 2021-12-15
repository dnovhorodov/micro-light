namespace MicroLight

[<AutoOpen>]
module Constants =
    let [<Literal>] DateTimeFormat = "dddd, dd MMMM yyyy HH:mm"
    let [<Literal>] LoggerDateTimeFormat = "MM/dd/yy H:mm:ss.fff zz"
    let [<Literal>] ContentAccessTopic = "test-topic"
    let [<Literal>] EmailRegexPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z"