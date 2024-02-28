using Ayleen.Broker.FileBroker.Models;
using Ayleen.Broker.Library.Interfaces;
using Ayleen.Broker.Library.Models;
using Microsoft.Extensions.Options;

namespace Ayleen.Broker.FileBroker.Services;

public class FileBroker(IOptions<FileBrokerSettings> options) : IBroker
{
    public async Task<bool> CreateRequestAsync(string hash, string key, CancellationToken cancellationToken)
    {
        var path = Path.Combine(options.Value.Path, options.Value.RequestFolder);
        if (!Path.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var filePath = Path.ChangeExtension(Path.Combine(path, hash), ".req");
        if (Path.Exists(filePath))
        {
            return false;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }

        await using var file = File.CreateText(filePath);
        await file.WriteLineAsync(key);
        file.Close();

        return true;
    }

    public async Task<BrokerContent?> GetResponseAsync(string hash, CancellationToken cancellationToken)
    {
        var path = Path.Combine(options.Value.Path, options.Value.ResponseFolder);
        if (!Path.Exists(path))
        {
            return null;
        }

        var filePath = Path.ChangeExtension(Path.Combine(path, hash), ".rsp");
        if (!Path.Exists(filePath))
        {
            return null;
        }

        var payload = await File.ReadAllTextAsync(filePath, cancellationToken);
        File.Delete(filePath);
        
        return new BrokerContent(payload);
    }
}