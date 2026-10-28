namespace NorthWind.Sales.Backend.Repositories.Repositories;

internal class CommandsRepository(INorthWindSalesCommandsDataContext context) :
    ICommandsRepository
{
    public async Task CreateOrder(OrderAggregate order)
    {
        foreach (var detail in order.OrderDetails)
            if (await context.DecreaseStockAsync(detail.ProductId, detail.Quantity) != 1)
                throw new ValidationException([new ValidationError(nameof(detail.Quantity), $"Insufficient stock for product {detail.ProductId}; refresh the product list and try again.")]);
        await context.AddOrderAsync(order);
        await context.AddOrderDetailsAsync(
            order.OrderDetails
                .Select(d => new Entities.OrderDetail
                {
                    Order = order,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToArray());
    }

    public async Task SaveChanges() =>
        await context.SaveChangesAsync();
}
