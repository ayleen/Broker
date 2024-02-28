using Ayleen.Broker.Api.Interfaces;
using Ayleen.Broker.Api.Models;
using Ayleen.Broker.Manager.Interfaces;
using Ayleen.Broker.Manager.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Ayleen.Broker.Api.Services;

public class CachedEventManager(IEventManager eventManager, IMemoryCache memoryCache, IOptions<CachedEventManagerSettings> options) : ICachedEventManager
{
    public Task<bool> AddEventAsync(string hash, string key, CancellationToken cancellationToken)
    {
        return eventManager.AddEventAsync(hash, key, cancellationToken);
    }

    public async Task<EventData?> GetEventAsync(string hash, string key, CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue<EventData>(GetCacheKey(hash, key), out var data))
        {
            if (data != null)
            {
                return data;
            }
        }

        var eventData = await eventManager.GetEventAsync(hash, key, cancellationToken);
        if (eventData != null)
        {
            memoryCache.Set(GetCacheKey(hash, key), eventData, TimeSpan.FromSeconds(options.Value.KeyTimeoutInSeconds));
            
        }
        
        return eventData;
    }

    private static string GetCacheKey(string hash, string key)
    {
        return $"{key}";
    }
}