using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Ecom.Contracts.Orders;
using Ecom.Contracts.Stocks;
using Microsoft.Extensions.Hosting.ServiceBus;

namespace Ecom.Api.Handlers;

public class RequestHandlers
{
    public static async Task<IResult> HandlePlaceOrderRequest(
        ILogger<RequestHandlers> logger,
        PlaceOrderRequest request,
        HttpClient httpClient,
        AzureServiceBusPublisher publisher)
    {
        Activity.Current?.SetTag(
            "customer.id",
            request.CustomerId);
    
        logger.LogInformation("Trying to place order for {CustomerId}", request.CustomerId);
    
        var response = await httpClient.PostAsync(
            "http://stock/api/check-stock",
            new StringContent(
                JsonSerializer.Serialize(new CheckStockRequest(request.OrderLines)),
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            return TypedResults.BadRequest("Unable to place order");
        }

        var orderId = Guid.NewGuid().ToString();
        var lines = new List<(string Id, string Sku)>();

        foreach (var requestLines in request.OrderLines)
        {
            for (var i = 0; i < requestLines.Count; i++)
            {
                lines.Add((Guid.NewGuid().ToString(), requestLines.Sku));
            }
        }

        await publisher.SendAsync(
            new OrderPlacedEvent(
                orderId,
                request.CustomerId,
                lines));

        logger.LogInformation("Placed order {OrderId} for {CustomerId}", orderId, request.CustomerId);
    
        return TypedResults.Ok();
    }
}