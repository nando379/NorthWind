namespace Microsoft.AspNetCore.Builder;

public static class SalesQueriesController
{
    public static WebApplication UseSalesQueriesController(this WebApplication app)
    {
        app.MapGet(Endpoints.SearchCustomers, async (string? search, string? field, int? page, int? pageSize, IQueriesRepository repository) =>
            Results.Ok(await repository.SearchCustomers(search, field, Math.Max(page ?? 1, 1), Math.Clamp(pageSize ?? 5, 1, 100))))
            .RequireAuthorization();
        app.MapGet(Endpoints.SearchProducts, async (string? search, string? field, int? page, int? pageSize, IQueriesRepository repository) =>
            Results.Ok(await repository.SearchProducts(search, field, Math.Max(page ?? 1, 1), Math.Clamp(pageSize ?? 5, 1, 100))))
            .RequireAuthorization();
        app.MapGet(Endpoints.SearchInvoices, async (string? search, string? field, int? page, int? pageSize, IQueriesRepository repository) =>
            Results.Ok(await repository.SearchInvoices(search, field, Math.Max(page ?? 1, 1), Math.Clamp(pageSize ?? 5, 1, 100))))
            .RequireAuthorization();
        app.MapGet(Endpoints.GetInvoice, async (int orderId, IQueriesRepository repository) =>
        {
            var invoice = await repository.GetInvoice(orderId);
            return invoice is null ? Results.NotFound() : Results.Ok(invoice);
        }).RequireAuthorization();
        app.MapGet(Endpoints.GetTaxRate, (ITaxRateProvider taxRateProvider) =>
            Results.Ok(taxRateProvider.CurrentRate)).RequireAuthorization();
        return app;
    }
}
