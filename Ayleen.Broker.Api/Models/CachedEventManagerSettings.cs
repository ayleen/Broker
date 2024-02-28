namespace Ayleen.Broker.Api.Models;

public class CachedEventManagerSettings
{
    public const string Section = "CachedEventManager";

    public int KeyTimeoutInSeconds { get; init; } = 600;
}