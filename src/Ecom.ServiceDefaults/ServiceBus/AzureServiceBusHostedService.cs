using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Ecom.ServiceDefaults.ServiceBus;

public class AzureServiceBusHostedService(
    ILogger<AzureServiceBusHostedService> logger,
    IEnumerable<IRootHandler> rootHandlers) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting Azure Service Bus Hosted Service");
        
        var tasks = rootHandlers.Select(x => x.StartAsync());
        
        await Task.WhenAll(tasks);
        
        logger.LogInformation("Started Azure Service Bus Hosted Service");
    }
}