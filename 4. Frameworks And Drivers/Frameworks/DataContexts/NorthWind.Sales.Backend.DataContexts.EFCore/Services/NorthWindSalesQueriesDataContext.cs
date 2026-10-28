namespace NorthWind.Sales.Backend.DataContexts.EFCore.Services;

internal class NorthWindSalesQueriesDataContext :
    NorthWindSalesContext,
    INorthWindSalesQueriesDataContext
{
    public NorthWindSalesQueriesDataContext(IOptions<DBOptions> dbOptions)
        : base(dbOptions)
    {
        ChangeTracker.QueryTrackingBehavior =
            QueryTrackingBehavior.NoTracking;
    }

    public new IQueryable<Customer> Customers => base.Customers;
    public new IQueryable<Product> Products => base.Products;
    public new IQueryable<Order> Orders => base.Orders;
    public new IQueryable<Repositories.Entities.OrderDetail> OrderDetails => base.OrderDetails;
    public Task<int> CountAsync<T>(IQueryable<T> queryable) => queryable.CountAsync();

    public Task<ReturnType> FirstOrDefaultAync<ReturnType>(
        IQueryable<ReturnType> queryable) =>
        queryable.FirstOrDefaultAsync();

    public async Task<IEnumerable<ReturnType>> ToListAsync<ReturnType>(
        IQueryable<ReturnType> queryable) =>
        await queryable.ToListAsync();
}
