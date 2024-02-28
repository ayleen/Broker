using Ayleen.Broker.Api.Models;

namespace Ayleen.Broker.Api.Interfaces;

public interface IDataService
{
    Task<GetDataResponse> GetResponseAsync(string hash, string key, CancellationToken cancellationToken);
}