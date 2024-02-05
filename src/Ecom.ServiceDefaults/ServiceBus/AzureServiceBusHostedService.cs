namespace Microsoft.Extensions.Hosting.ServiceBus;

public class AzureServiceBusHostedService(IEnumerable<IRootHandler> rootHandlers) : BackgroundService
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var tasks = rootHandlers.Select(x => x.StartAsync());
        return Task.WhenAll(tasks);
    }
}