using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DotnetTestContainers;

public class Function1(ILogger<Function1> logger)
{
    private readonly ILogger<Function1> _logger = logger;

    [Function(nameof(Function1))]
    public void Run([QueueTrigger("samplequeueitems")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {MessageText}", message.MessageText);
    }
}
