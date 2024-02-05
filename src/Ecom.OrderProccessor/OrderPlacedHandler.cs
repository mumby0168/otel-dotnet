using Azure.Messaging.ServiceBus;
using Ecom.Contracts.Orders;
using Microsoft.Extensions.Hosting.ServiceBus;

namespace Ecom.OrderProccessor;

public class OrderPlacedHandler(ServiceBusClient client, ILogger<OrderPlacedHandler> logger) : MessageHandlerBase<OrderPlacedEvent>(
    client,
    logger)
{
    private readonly ILogger _logger = logger;

    public override Task HandleAsync(
        OrderPlacedEvent message)
    {
        _logger.LogInformation(
            "Order successfully placed for {CustomerId} with {OrderId}",
            message.CustomerId,
            message.Id);

        return Task.CompletedTask;
    }
}