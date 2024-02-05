using Ecom.Contracts.Orders;
using Ecom.OrderProccessor;
using Ecom.ServiceDefaults.ServiceBus;
using Microsoft.Extensions.Hosting.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureServiceBus("serviceBus");
builder.Services.AddHostedService<AzureServiceBusHostedService>();
builder.Services.AddSingleton<IMessageHandler<OrderPlacedEvent>, OrderPlacedHandler>();
builder.Services.AddSingleton<IRootHandler, OrderPlacedHandler>();
builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet(
    "/",
    () => "OrderProcessor");

app.Run();