using Ecom.AppHost;
using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos");
var serviceBus = builder.AddAzureServiceBus("serviceBus");

var stockApi = builder.AddProject<Ecom_StockApi>("stock")
    .WithReference(cosmos)
    .AddEnvironmentVariablesForOtelExporters(builder.Configuration);

builder.AddProject<Ecom_Api>("gateway")
    .WithReference(serviceBus)
    .WithReference(stockApi)
    .AddEnvironmentVariablesForOtelExporters(builder.Configuration);

builder.AddProject<Ecom_OrderProccessor>("orders")
    .WithReference(serviceBus)
    .WithReference(stockApi)
    .AddEnvironmentVariablesForOtelExporters(builder.Configuration);

builder.Build().Run();

