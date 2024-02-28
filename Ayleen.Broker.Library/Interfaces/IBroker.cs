using Ayleen.Broker.Library.Models;

namespace Ayleen.Broker.Library.Interfaces;

public interface IBroker
{
    Task<bool> CreateRequestAsync(string hash, string key, CancellationToken cancellationToken);

    Task<BrokerContent?> GetResponseAsync(string hash, CancellationToken cancellationToken);
}