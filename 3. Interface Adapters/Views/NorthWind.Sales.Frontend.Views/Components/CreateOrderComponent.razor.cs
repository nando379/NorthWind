using System.Globalization;

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
    private ElementReference CustomerSearchInput;
    private ElementReference ProductSearchInput;

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
        Order.SearchText = FilterCustomerSearchText(rawInput, Order.CustomerFilter);
        await CorrectVisibleInputAsync(CustomerSearchInput, rawInput, Order.SearchText);
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
        Order.SearchText = FilterCustomerSearchText(Order.SearchText, Order.CustomerFilter);
        await SetVisibleInputAsync(CustomerSearchInput, Order.SearchText);
        await SearchNow();
    }

    private static string FilterCustomerSearchText(string value, string filter)
    {
        Func<char, bool> allowed = filter switch
        {
            "ID" => char.IsLetterOrDigit,
            "Nombre" => char.IsLetter,
            "Todas" => char.IsLetterOrDigit,
            _ => char.IsLetterOrDigit
        };

        return new string(value
            .Where(character => allowed(character) ||
                                ((filter is "Nombre" or "Todas") && character == ' '))
            .Take(40)
            .ToArray());
    }

    async Task OnProductSearchInput(ChangeEventArgs e)
    {
        var rawInput = e.Value?.ToString() ?? string.Empty;
        Order.SearchText = FilterProductSearchText(rawInput, Order.ProductFilter);
        await CorrectVisibleInputAsync(ProductSearchInput, rawInput, Order.SearchText);
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
        Order.SearchText = FilterProductSearchText(Order.SearchText, Order.ProductFilter);
        await SetVisibleInputAsync(ProductSearchInput, Order.SearchText);
        await SearchNow();
    }

    private async Task CorrectVisibleInputAsync(ElementReference input, string rawValue, string filteredValue)
    {
        if (!string.Equals(rawValue, filteredValue, StringComparison.Ordinal))
        {
            await SetVisibleInputAsync(input, filteredValue);
        }
    }

    private Task SetVisibleInputAsync(ElementReference input, string value) =>
        JavaScript.InvokeVoidAsync("northWindInputFilter.setValue", input, value).AsTask();

    private static string FilterProductSearchText(string value, string filter)
    {
        Func<char, bool> allowed = filter switch
        {
            "ID" or "Stock" or "Precio" => char.IsDigit,
            "Nombre" or "Todas" => char.IsLetterOrDigit,
            _ => char.IsLetterOrDigit
        };

        return new string(value
            .Where(character => allowed(character) ||
                                ((filter is "Nombre" or "Todas") && character == ' '))
            .Take(40)
            .ToArray());
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
