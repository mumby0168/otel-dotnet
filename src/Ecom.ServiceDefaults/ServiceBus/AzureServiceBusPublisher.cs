using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Ecom.Contracts;

namespace Microsoft.Extensions.Hosting.ServiceBus;

public class AzureServiceBusPublisher(ServiceBusClient client)
{
    private readonly ServiceBusClient _client = client;
    private static readonly Dictionary<string, ServiceBusSender> Senders = new();
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task SendAsync<T>(
        T orderPlacedEvent) where T : IEvent
    {
        if (!Senders.ContainsKey(T.Topic))
        {
            Senders[T.Topic] = _client.CreateSender(T.Topic);
        }

        var sender = Senders[T.Topic];

        var message = new ServiceBusMessage(
            JsonSerializer.Serialize(
                orderPlacedEvent,
                _options));

        await sender.SendMessageAsync(message);
    }
}