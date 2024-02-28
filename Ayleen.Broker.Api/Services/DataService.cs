using System.Collections.Concurrent;
using Ayleen.Broker.Api.Interfaces;
using Ayleen.Broker.Api.Models;
using Ayleen.Broker.Manager.Models;
using Microsoft.Extensions.Options;

namespace Ayleen.Broker.Api.Services;

public class DataService(ICachedEventManager eventManager, IOptions<DataServiceSettings> options) : IDataService
{
    private static readonly ConcurrentDictionary<string, DateTime> KeyRequests = new();
    
    public async Task<GetDataResponse> GetResponseAsync(string hash, string key,
        CancellationToken cancellationToken)
    {
        if (KeyRequests.ContainsKey(key) && KeyRequests.TryGetValue(key, out var dateTime))
        {
            if (DateTime.Now - dateTime > TimeSpan.FromSeconds(options.Value.KeyTimeoutInSeconds))
            {
                KeyRequests.Remove(key, out _);
            }
        }
        
        if (!KeyRequests.ContainsKey(key))
        {
            if (KeyRequests.TryAdd(key, DateTime.Now))
            {
                await eventManager.AddEventAsync(hash, key, cancellationToken);
            }
        }
        
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            var eventData = await eventManager.GetEventAsync(hash, key, cancellationToken);
            if (eventData != null)
            {
                KeyRequests.Remove(key, out _);
                return ConvertEventDataToResponse(eventData);
            }

            await Task.Delay(100, cancellationToken);
        }
    }

    private static GetDataResponse ConvertEventDataToResponse(EventData eventData)
    {
        return new GetDataResponse(eventData.Key, eventData.Payload);
    }
}