using Ayleen.Broker.Manager.Models;

namespace Ayleen.Broker.Manager.Interfaces;

public interface IEventManager
{
    Task<bool> AddEventAsync(string hash, string key, CancellationToken cancellationToken);

    Task<EventData?> GetEventAsync(string hash, string key, CancellationToken cancellationToken);
}