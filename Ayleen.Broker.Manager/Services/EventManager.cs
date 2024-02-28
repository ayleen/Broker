using Ayleen.Broker.Library.Interfaces;
using Ayleen.Broker.Manager.Interfaces;
using Ayleen.Broker.Manager.Models;

namespace Ayleen.Broker.Manager.Services;

public class EventManager(IBroker broker) : IEventManager
{
    public Task<bool> AddEventAsync(string hash, string key, CancellationToken cancellationToken)
    {
        return broker.CreateRequestAsync(hash, key, cancellationToken);
    }

    public async Task<EventData?> GetEventAsync(string hash, string key, CancellationToken cancellationToken)
    {
        var content = await broker.GetResponseAsync(hash, cancellationToken);
        if (content == null)
        {
            return null;
        }

        return new EventData
        {
            Key = key,
            Payload = content.Payload
        };
    }
}