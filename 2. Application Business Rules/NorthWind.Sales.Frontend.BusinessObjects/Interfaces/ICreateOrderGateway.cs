namespace NorthWind.Sales.Frontend.BusinessObjects.Interfaces;

public interface ICreateOrderGateway
{
    Task<int> CreateOrderAsync(CreateOrderDto order);
    Task<SearchPageDto<CustomerSearchItemDto>> SearchCustomersAsync(string? search, string? field, int page, int pageSize);
    Task<SearchPageDto<ProductSearchItemDto>> SearchProductsAsync(string? search, string? field, int page, int pageSize);
    Task<SearchPageDto<InvoiceSearchItemDto>> SearchInvoicesAsync(string? search, string? field, int page, int pageSize);
    Task<InvoiceDto?> GetInvoiceAsync(int orderId);
    Task<decimal> GetTaxRateAsync();
}
