namespace Microsoft.Extensions.Hosting.Cosmos;

public interface ICosmosItem
{
    abstract static string ContainerName { get; }
    
    string PartitionKey { get; }
}