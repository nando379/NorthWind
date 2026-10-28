namespace NorthWind.Sales.Frontend.WebApiGateways;

internal class CreateOrderGateway(HttpClient client) : ICreateOrderGateway
{
    public async Task<int> CreateOrderAsync(CreateOrderDto order)
    {
        var Response = await client.PostAsJsonAsync(
            Endpoints.CreateOrder, order);
        if (!Response.IsSuccessStatusCode)
        {
            var error = await Response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(error)
                    ? $"The order could not be saved (HTTP {(int)Response.StatusCode})."
                    : error,
                null,
                Response.StatusCode);
        }
        return await Response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<SearchPageDto<CustomerSearchItemDto>> SearchCustomersAsync(string? search, string? field, int page, int pageSize) =>
        await client.GetFromJsonAsync<SearchPageDto<CustomerSearchItemDto>>($"{Endpoints.SearchCustomers}?search={Uri.EscapeDataString(search ?? string.Empty)}&field={Uri.EscapeDataString(field ?? string.Empty)}&page={page}&pageSize={pageSize}") ?? new([], page, pageSize, 0);

    public async Task<SearchPageDto<ProductSearchItemDto>> SearchProductsAsync(string? search, string? field, int page, int pageSize) =>
        await client.GetFromJsonAsync<SearchPageDto<ProductSearchItemDto>>($"{Endpoints.SearchProducts}?search={Uri.EscapeDataString(search ?? string.Empty)}&field={Uri.EscapeDataString(field ?? string.Empty)}&page={page}&pageSize={pageSize}") ?? new([], page, pageSize, 0);

    public async Task<SearchPageDto<InvoiceSearchItemDto>> SearchInvoicesAsync(string? search, string? field, int page, int pageSize) =>
        await client.GetFromJsonAsync<SearchPageDto<InvoiceSearchItemDto>>($"{Endpoints.SearchInvoices}?search={Uri.EscapeDataString(search ?? string.Empty)}&field={Uri.EscapeDataString(field ?? string.Empty)}&page={page}&pageSize={pageSize}") ?? new([], page, pageSize, 0);

    public Task<InvoiceDto?> GetInvoiceAsync(int orderId) =>
        client.GetFromJsonAsync<InvoiceDto>($"/invoices/{orderId}");

    public async Task<decimal> GetTaxRateAsync() =>
        await client.GetFromJsonAsync<decimal>(Endpoints.GetTaxRate);
}
