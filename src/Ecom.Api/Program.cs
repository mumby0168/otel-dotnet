using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Ecom.Api.Handlers;
using Ecom.Contracts.Orders;
using Ecom.Contracts.Stocks;
using Microsoft.Extensions.Hosting.ServiceBus;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwaggerGen(options => { options.CustomSchemaIds(type => type.FullName); });

builder.Services.AddEndpointsApiExplorer();

builder.AddAzureServiceBus("serviceBus");
builder.AddServiceDefaults();
builder.Services.AddSingleton<AzureServiceBusPublisher>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints();

app.MapGet(
    "/",
    () => "Ecom.Api");

app.MapPost(
    "/api/orders",
    RequestHandlers.HandlePlaceOrderRequest);

app.Run();
return;