using Ayleen.Broker.FileBroker.Models;
using Microsoft.Extensions.Options;
using Quartz;

namespace Ayleen.Broker.FileBroker.Services;

public class SchedulerJob(IOptions<FileBrokerSettings> options) : IJob
{
    public static readonly JobKey Key = new("file-scheduler-job", "broker");
    
    public async Task Execute(IJobExecutionContext context)
    {
        var requestsPath = Path.Combine(options.Value.Path, options.Value.RequestFolder);
        if (!Path.Exists(requestsPath))
        {
            return;
        }
        var responsesPath = Path.Combine(options.Value.Path, options.Value.ResponseFolder);
        if (!Path.Exists(responsesPath))
        {
            Directory.CreateDirectory(responsesPath);
        }

        var requestsDirectory = new DirectoryInfo(requestsPath);
        var files = requestsDirectory.GetFiles("*.req.*", SearchOption.TopDirectoryOnly);
        var fileNames = new HashSet<string>(files.Select(f => f.Name));

        var filesToProcess = 
            (
                from fileName in 
                    fileNames.Where(f 
                        => f.EndsWith(".req", StringComparison.InvariantCultureIgnoreCase)) 
                where !fileNames.Contains(fileName + ".lck") 
                select fileName
            ).ToList();

        using var semaphore = new SemaphoreSlim(10);
        var taskList = new List<Task>();
        foreach (var fileName in filesToProcess)
        {
            await semaphore.WaitAsync();
            var task = Task.Run(async () =>
            {
                try
                {
                    var lockFileName = Path.Combine(requestsPath, fileName + ".lck");
                    var fileLck = File.Create(lockFileName);
                    fileLck.Close();

                    var requestFileName = Path.Combine(requestsPath, fileName);
                    var key = await File.ReadAllTextAsync(requestFileName);

                    var responseFileName = Path.ChangeExtension(Path.Combine(responsesPath, fileName), ".rsp");
                    await using var responseFile =File.CreateText(responseFileName);
                    var response = DataCalculator.Calculate(key);
                    await responseFile.WriteAsync($"200\n{response}");
                    responseFile.Close();
            
                    File.Delete(lockFileName);
                    File.Delete(requestFileName);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            taskList.Add(task);
        }

        await Task.WhenAll(taskList);
    }
}