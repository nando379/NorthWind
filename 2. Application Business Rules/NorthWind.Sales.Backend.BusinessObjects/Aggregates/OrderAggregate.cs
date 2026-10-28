namespace NorthWind.Sales.Backend.BusinessObjects.Aggregates;

public class OrderAggregate : Order
{
    readonly List<OrderDetail> OrderDetailsField = [];
    public IReadOnlyCollection<OrderDetail> OrderDetails => OrderDetailsField;

    public void AddDetail(int productId, decimal unitPrice, short quantity)
    {
        var ExistingOrderDetail = OrderDetailsField.FirstOrDefault(o => o.ProductId == productId);
        if (ExistingOrderDetail != default)
            throw new ValidationException([new ValidationError(nameof(CreateOrderDto.OrderDetails), $"Product {productId} is already in the order.")]);
        if (quantity <= 0 || unitPrice < 0)
            throw new ValidationException([new ValidationError(nameof(CreateOrderDto.OrderDetails), "Product quantity must be positive and price cannot be negative.")]);
        OrderDetailsField.Add(new OrderDetail(productId, unitPrice, quantity));
    }

    public static OrderAggregate From(CreateOrderDto orderDto, decimal taxRate)
    {
        OrderAggregate OrderAggregate = new OrderAggregate
        {
            CustomerId = orderDto.CustomerId,
            ShipAddress = orderDto.ShipAddress,
            ShipCity = orderDto.ShipCity,
            ShipCountry = orderDto.ShipCountry,
            ShipPostalCode = orderDto.ShipPostalCode,
            TaxRate = taxRate
        };

        foreach (var Item in orderDto.OrderDetails)
        {
            OrderAggregate.AddDetail(Item.ProductId, Item.UnitPrice, Item.Quantity);
        }

        return OrderAggregate;
    }
}
