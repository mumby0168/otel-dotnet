using Microsoft.Extensions.Hosting.Cosmos;

namespace Ecom.StockApi.Data;

public class StockRecord : ICosmosItem
{
    public static string ContainerName => "stock";
    public string Id { get; }
    public string PartitionKey { get; }
    
    public string Sku { get; set; }
    
    public int Count { get; set; }

    public StockRecord(string sku, int count)
    {
        Sku = sku;
        Count = count;
        PartitionKey = Sku;
        Id = Sku;
    }
}