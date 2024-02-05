using Ecom.Contracts.Orders;
using Microsoft.Extensions.Hosting.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet(
    "/",
    () => "Ecom.Api");

app.MapPost(
    "/api/orders", HandlePlaceOrderRequest);

app.Run();
return;

static async Task<IResult> HandlePlaceOrderRequest(
    PlaceOrderRequest request, 
    HttpClient httpClient,
    AzureServiceBusPublisher publisher)
{
    var response = await httpClient.GetAsync("stock/api/validate-holdings");

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

    return TypedResults.Ok();
}