namespace Ayleen.Broker.Api.Models;

public class DataServiceSettings
{
    public const string Section = "DataService";

    public int KeyTimeoutInSeconds { get; init; } = 600;
}