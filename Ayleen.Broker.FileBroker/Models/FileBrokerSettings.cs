namespace Ayleen.Broker.FileBroker.Models;

public class FileBrokerSettings
{
    public const string Section = "FileBroker";

    public string Path { get; init; } = System.IO.Path.GetTempPath();

    public string RequestFolder { get; init; } = "Requests";

    public string ResponseFolder { get; init; } = "Response";
}