namespace Ecom.Contracts.Orders;

public record PlaceOrderRequest(
    string CustomerId,
    List<(string Sku, int Count)> OrderLines);