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
    static abstract string Topic { get; }
    
    Task HandleAsync(T message);
}

public abstract class MessageHandlerBase<T>(ServiceBusClient client, ILogger logger) : IMessageHandler<T> where T : IEvent
{
    public static string Topic { get; }
    public abstract Task HandleAsync(T message);
    
    public Task StartAsync()
    {
        var processor = client.CreateProcessor(Topic);
        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;
        return processor.StartProcessingAsync();
    }
    
    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
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