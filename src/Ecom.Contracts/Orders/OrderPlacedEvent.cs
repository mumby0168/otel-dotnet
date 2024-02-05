namespace Ecom.Contracts.Orders;

public record OrderPlacedEvent(
    string Id,
    string CustomerId,
    List<(string Id, string Sku)> Lines) : IEvent
{
    public static string Topic => "ecom-api/order-placed";
}