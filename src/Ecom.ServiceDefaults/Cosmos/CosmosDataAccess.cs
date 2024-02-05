using Microsoft.Azure.Cosmos;

namespace Microsoft.Extensions.Hosting.Cosmos;

public class CosmosDataAccess(CosmosClient cosmosClient)
{
    private readonly CosmosClient _cosmosClient = cosmosClient;
    private static readonly Dictionary<string, Container> Senders = new();

    private async Task<Container> GetOrCreateCosmosContainer<T>() where T : ICosmosItem
    {
        if (Senders.TryGetValue(
                T.ContainerName,
                out Container? value))
        {
            return value;
        }

        await _cosmosClient.CreateDatabaseIfNotExistsAsync("ecom");

        await _cosmosClient.GetDatabase("ecom").CreateContainerIfNotExistsAsync(
            new ContainerProperties(
                T.ContainerName,
                "/partitionKey"));

        Senders[T.ContainerName] = _cosmosClient.GetContainer(
            "ecom",
            T.ContainerName);


        return Senders[T.ContainerName];
    }
    
    public async Task<T> GetItemAsync<T>(
        string id,
        string partitionKey) where T : ICosmosItem
    {
        var container = await GetOrCreateCosmosContainer<T>();

        var response = await container.ReadItemAsync<T>(
            id,
            new PartitionKey(partitionKey));

        return response.Resource;
    }
    
    public async Task CreateItemAsync<T>(
        T item) where T : ICosmosItem
    {
        var container = await GetOrCreateCosmosContainer<T>();

        await container.CreateItemAsync(
            item,
            new PartitionKey(item.PartitionKey));
    }
}