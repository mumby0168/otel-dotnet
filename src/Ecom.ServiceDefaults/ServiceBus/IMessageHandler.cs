using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Ecom.Contracts;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting.ServiceBus;

public interface IRootHandler
{
    Task StartAsync();
}

public interface IMessageHandler<in T> : IRootHandler where T : IEvent
{
    
    Task HandleAsync(T message);
}

public abstract class MessageHandlerBase<T>(ServiceBusClient client, ILogger<IMessageHandler<T>> logger) : IMessageHandler<T> where T : IEvent
{
    public abstract Task HandleAsync(T message);
    
    public async Task StartAsync()
    {
        var processor = client.CreateProcessor(T.Topic, "example-sub");
        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;
        logger.LogInformation("Starting processing messages for {Topic}", T.Topic);
        await processor.StartProcessingAsync();
        logger.LogInformation("Started processing messages for {Topic}", T.Topic);
    }
    
    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        logger.LogInformation("Processing message: {Message}", args.Message.MessageId);
        
        try
        {
            var body = args.Message.Body.ToString();
            var message = JsonSerializer.Deserialize<T>(body)!;
            await HandleAsync(message);
        }
        catch (Exception e)
        {
            await args.DeadLetterMessageAsync(
                args.Message,
                e.GetType().Name,
                e.Message);
        }
        
        
        await args.CompleteMessageAsync(args.Message);
    }
    
    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(
            args.Exception,
            "Error processing message: {Message}",
            args.Exception.Message);
        return Task.CompletedTask;
    }
}