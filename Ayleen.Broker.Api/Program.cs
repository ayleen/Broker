using Ayleen.Broker.Api.Interfaces;
using Ayleen.Broker.Api.Models;
using Ayleen.Broker.Api.Services;
using Ayleen.Broker.FileBroker.Models;
using Ayleen.Broker.FileBroker.Services;
using Ayleen.Broker.Library.Interfaces;
using Ayleen.Broker.Manager.Interfaces;
using Ayleen.Broker.Manager.Services;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services
    .AddTransient<IDataService, DataService>()
    .AddTransient<IEventManager, EventManager>()
    .AddTransient<ICachedEventManager, CachedEventManager>()
    .AddSingleton<IBroker, FileBroker>();

builder.Services
    .Configure<FileBrokerSettings>(builder.Configuration.GetSection(FileBrokerSettings.Section))
    .Configure<DataServiceSettings>(builder.Configuration.GetSection(DataServiceSettings.Section))
    .Configure<CachedEventManagerSettings>(builder.Configuration.GetSection(CachedEventManagerSettings.Section));

builder.Services
    .AddQuartz(q =>
    {
        q.AddJob<SchedulerJob>(o => o.WithIdentity(SchedulerJob.Key));
        q.AddTrigger(o =>
            o.ForJob(SchedulerJob.Key)
                .WithIdentity($"{SchedulerJob.Key}-trigger")
                .WithCronSchedule("*/10 * * ? * *")
        );
    })
    .AddQuartzServer(options =>
    {
        options.WaitForJobsToComplete = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getdata/{key}", async ([FromServices] IDataService data, [FromRoute] string key, CancellationToken cancellationToken) 
        => Results.Ok(await data.GetResponseAsync(Helpers.GetMd5("/getData"), key, cancellationToken)))
    .WithName("GetData")
    .WithOpenApi();

app.Run();