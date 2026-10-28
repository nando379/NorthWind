namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;

public interface IQueriesRepository
{
    Task<decimal?> GetCustomerCurrentBalance(string customerId);
    Task<IEnumerable<ProductUnitsInStock>>
        GetProductsUnitsInStock(IEnumerable<int> productIds);
    Task<SearchPageDto<CustomerSearchItemDto>> SearchCustomers(string? search, string? field, int page, int pageSize);
    Task<SearchPageDto<ProductSearchItemDto>> SearchProducts(string? search, string? field, int page, int pageSize);
    Task<SearchPageDto<InvoiceSearchItemDto>> SearchInvoices(string? search, string? field, int page, int pageSize);
    Task<InvoiceDto?> GetInvoice(int orderId);
}
