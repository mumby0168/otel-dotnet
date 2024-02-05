namespace Ecom.Contracts.Stocks;

public record CheckStockRequest(
    List<(string Sku, int Count)> RequiredStock);