namespace Ayleen.Broker.Api.Models;

public class GetDataResponse(string key, string payload)
{
    public string Key { get; set; } = key;

    public string Payload { get; set; } = payload;
}