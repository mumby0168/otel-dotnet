using Ecom.Contracts.Stocks;

namespace Ecom.Contracts.Orders;

public record PlaceOrderRequest(
    string CustomerId,
    List<StockRequestItem> OrderLines);