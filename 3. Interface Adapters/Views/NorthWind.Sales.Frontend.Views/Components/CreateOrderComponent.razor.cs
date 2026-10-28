using System.Globalization;
using System.Text.RegularExpressions;

namespace NorthWind.Sales.Frontend.Views.Components;

public partial class CreateOrderComponent : IDisposable
{
    private static readonly CultureInfo EcCulture = CultureInfo.GetCultureInfo("es-EC");

    private string CustomerSearchPlaceholder => Order.CustomerFilter switch
    {
        "ID" => "Buscar por ID...",
        "Nombre" => "Buscar por nombre (solo letras)...",
        _ => "Buscar por ID o nombre..."
    };

    private static string FormatCurrency(decimal amount) =>
        $"${amount.ToString("N2", CultureInfo.InvariantCulture)}";

    private static string FormatDecimal(decimal amount) =>
        amount.ToString("N2", EcCulture);

    private CancellationTokenSource? _searchCts;

    [Parameter]
    public CreateOrderViewModel Order { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Order.TaxRate = await Order.LoadTaxRateAsync();
        }
        catch
        {
            Order.TaxRate = 0.15m;
        }
    }

    async Task OpenCustomerSearch()
    {
        Order.OpenCustomerSearch();
        await Order.LoadPageAsync();
    }

    async Task OpenProductSearch()
    {
        Order.OpenProductSearch();
        await Order.LoadPageAsync();
    }

    async Task OnCustomerSearchInput(ChangeEventArgs e)
    {
        var rawInput = e.Value?.ToString() ?? string.Empty;
        if (Order.CustomerFilter == "Nombre")
        {
            rawInput = Regex.Replace(rawInput, @"[^a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]", "");
        }
        else if (Order.CustomerFilter == "ID")
        {
            rawInput = Regex.Replace(rawInput, @"[^a-zA-Z0-9]", "");
        }

        Order.SearchText = rawInput;
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            await Task.Delay(100, token);
            await Order.SearchAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (TaskCanceledException)
        {
        }
    }

    async Task OnCustomerFilterChanged(ChangeEventArgs e)
    {
        Order.CustomerFilter = e.Value?.ToString() ?? "Todas";
        if (Order.CustomerFilter == "Nombre" && !string.IsNullOrEmpty(Order.SearchText))
        {
            Order.SearchText = Regex.Replace(Order.SearchText, @"[^a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]", "");
        }
        else if (Order.CustomerFilter == "ID" && !string.IsNullOrEmpty(Order.SearchText))
        {
            Order.SearchText = Regex.Replace(Order.SearchText, @"[^a-zA-Z0-9]", "");
        }
        await SearchNow();
    }

    async Task OnProductSearchInput(ChangeEventArgs e)
    {
        Order.SearchText = e.Value?.ToString() ?? string.Empty;
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            await Task.Delay(100, token);
            await Order.SearchAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (TaskCanceledException)
        {
        }
    }

    async Task OnProductFilterChanged(ChangeEventArgs e)
    {
        Order.ProductFilter = e.Value?.ToString() ?? "Todas";
        await SearchNow();
    }

    async Task SearchNow()
    {
        _searchCts?.Cancel();
        await Order.SearchAsync();
        await InvokeAsync(StateHasChanged);
    }

    async Task ChangePage(int delta) => await Order.ChangePageAsync(delta);
    async Task GoToPage(int page) => await Order.GoToPageAsync(page);
    async Task PrintInvoice() => await JavaScript.InvokeVoidAsync("window.print");
    void SelectCustomer(CustomerSearchItemDto customer) => Order.SelectCustomer(customer);
    void AddProduct(ProductSearchItemDto product) => Order.AddProduct(product);

    async Task SearchOnEnter(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await SearchNow();
        }
    }

    public void Dispose()
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
    }
}
