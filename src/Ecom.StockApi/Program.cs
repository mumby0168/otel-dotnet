using Ecom.Contracts.Stocks;
using Ecom.StockApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddEndpointsApiExplorer();

builder.AddAzureCosmosDB("cosmos");
builder.Services.AddSingleton<CosmosDataAccess>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints();

app.MapGet(
    "/",
    () => "Ecom.StockApi");

app.MapPost("api/check-stock", HandleCheckStockRequest);

app.Run();
return;

static async Task<IResult> HandleCheckStockRequest(
    [FromBody] CheckStockRequest request,
    [FromServices] CosmosDataAccess cosmosDataAccess)
{
    foreach (var (sku, count) in request.RequiredStock)
    {
        var stockRecord = await cosmosDataAccess.GetItemAsync<StockRecord>(
            sku,
            sku);

        if (stockRecord.Count < count)
        {
            return TypedResults.BadRequest($"Insufficient stock for {sku}");
        }
    }

    return TypedResults.Ok();
}