namespace NorthWind.Sales.Backend.DataContexts.EFCore.Services;

internal class NorthWindSalesCommandsDataContext(
    IOptions<DBOptions> dbOptions) :
    NorthWindSalesContext(dbOptions),
    INorthWindSalesCommandsDataContext
{
    public async Task AddOrderAsync(Order order) =>
        await AddAsync(order);

    public async Task AddOrderDetailsAsync(
        IEnumerable<Repositories.Entities.OrderDetail> orderDetails) =>
        await AddRangeAsync(orderDetails);

    public Task<int> DecreaseStockAsync(int productId, short quantity) =>
        Products.Where(product => product.Id == productId && product.UnitsInStock >= quantity)
            .ExecuteUpdateAsync(setters => setters.SetProperty(
                product => product.UnitsInStock,
                product => (short)(product.UnitsInStock - quantity)));

    public async Task SaveChangesAsync() =>
        await GuardDBContext.AgainstSaveChangesErrorAsync(this);
}
