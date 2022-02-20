using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SevenStones;
using SevenStones.Processors;
using SevenStones.Services;
using AzureDevOpsEvents = SevenStones.Models.Microsoft;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SevenStones.Models.Microsoft;

//string connectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING");
string serviceBusConnectionString = "Endpoint=sb://ups-devsecops.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=x3jqNa+zenWrBceAOPJDUeDZHXJjY+0+6RjDQSwtigM=";
string queueName = Environment.GetEnvironmentVariable("SERVICEBUS_QUEUE_NAME") ?? "repo-commit-queue";
IServiceProvider services = null;
await using (var client = new ServiceBusClient(serviceBusConnectionString))
await using (var processor = client.CreateProcessor(queueName))
{

    processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
    processor.ProcessMessageAsync += Processor_ProcessMessageAsync;


    services = new ServiceCollection()
        .AddDbContext<DataContext>()
        .AddTransient<IRepositoryService, RepositoryService>()
        .AddTransient<DotNetVersionScanner>()
        .AddTransient<GitLeaksProcessor>()
        .AddLogging(config => config.AddSimpleConsole())
        .BuildServiceProvider();

    var logger = services.GetService<ILogger<AzureDevOpsEvent>>();

    logger.LogInformation($"Ensuring initial database schema is created..");

    var _dataContext = services.GetService<DataContext>(); 
    _dataContext.Database.EnsureCreated();


    logger.LogInformation($"Listening for work..");
    
    await processor.StartProcessingAsync();

    while (!processor.IsClosed)
    {
        await Task.Delay(1000);
    }

    logger.LogInformation($"Worker shutting down..");

}

async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs evt)
{

    var logger = services.GetService<ILogger<AzureDevOpsEvent>>();

    logger.LogInformation($"[{evt.Message.MessageId}] Received message with correlation id {evt.Message.CorrelationId}");
    var repoSvc = services.GetService<IRepositoryService>();
    var raw = evt.Message.Body.ToString();
    
    var message = JsonSerializer.Deserialize<AzureDevOpsEvent>(raw);
    logger.LogInformation($"[{evt.Message.MessageId}] Deserialized message of type {message.EventType}");
    try
    {
        foreach (var refUpdate in message.Resource.RefUpdates)
        {
            await repoSvc.ScanRepository(message.ResourceContainers.Collection.Id, message.Resource.Repository.Id,
                message.Resource.Repository.Name, message.Resource.Repository.RemoteUrl, message.Resource.Url,
                refUpdate.NewObjectId, refUpdate.Name, message.Resource.Date, message.Resource.Commits.FirstOrDefault()?.Url);
        }

    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"[{evt.Message.MessageId}] Failed to process message. \r\n Error: {ex.Message}");
        //await evt.DeadLetterMessageAsync(evt.Message);
        await evt.AbandonMessageAsync(evt.Message);
    }
    
}

Task Processor_ProcessErrorAsync(ProcessErrorEventArgs evt)
{
    throw new Exception($"Failed to process event: {evt.Exception.Message}", evt.Exception);
}