namespace Ecom.Contracts.Stocks;

public record CheckStockRequest(
    List<StockRequestItem> RequiredStock);
public record StockRequestItem(string Sku, int Count);