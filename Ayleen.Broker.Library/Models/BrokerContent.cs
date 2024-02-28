namespace Ayleen.Broker.Library.Models;

public class BrokerContent(string payload)
{
    public string Payload { get; set; } = payload;
}