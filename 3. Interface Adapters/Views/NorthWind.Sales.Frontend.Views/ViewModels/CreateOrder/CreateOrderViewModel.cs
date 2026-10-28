using System.Globalization;

namespace NorthWind.Sales.Frontend.Views.ViewModels.CreateOrder;

public class CreateOrderViewModel(ICreateOrderGateway gateway,
    IModelValidatorHub<CreateOrderViewModel> validator)
{
    public const int PageSize = 5;

    private string _shipAddress = string.Empty;
    private string _shipCity = string.Empty;
    private string _shipCountry = string.Empty;
    private string _shipPostalCode = string.Empty;

    public IModelValidatorHub<CreateOrderViewModel> Validator => validator;
    public ModelValidator<CreateOrderViewModel>? ModelValidatorComponentReference { get; set; }

    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal CustomerBalance { get; set; }
    public string ShipAddress
    {
        get => _shipAddress;
        set => _shipAddress = FilterInput(value, 60,
            character => char.IsLetter(character) || char.IsDigit(character) || character == ' ');
    }

    public string ShipCity
    {
        get => _shipCity;
        set => _shipCity = FilterInput(value, 15,
            character => char.IsLetter(character) || character == ' ');
    }

    public string ShipCountry
    {
        get => _shipCountry;
        set => _shipCountry = FilterInput(value, 15,
            character => char.IsLetter(character) || character == ' ');
    }

    public string ShipPostalCode
    {
        get => _shipPostalCode;
        set => _shipPostalCode = FilterInput(value, 10, char.IsLetterOrDigit);
    }

    private static string FilterInput(string? value, int maxLength, Func<char, bool> isAllowed) =>
        new((value ?? string.Empty).Where(isAllowed).Take(maxLength).ToArray());

    public List<CreateOrderDetailViewModel> OrderDetails { get; set; } = [];
    public string InformationMessage { get; set; } = string.Empty;
    public string WarningMessage { get; set; } = string.Empty;

    public string SearchText { get; set; } = string.Empty;
    public string CustomerFilter { get; set; } = "Todas";
    public string ProductFilter { get; set; } = "Todas";

    public IReadOnlyList<CustomerSearchItemDto> Customers { get; private set; } = [];
    public IReadOnlyList<ProductSearchItemDto> Products { get; private set; } = [];
    public InvoiceDto? CreatedInvoice { get; private set; }

    public int Page { get; private set; } = 1;
    public int TotalCount { get; private set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));

    public bool IsCustomerModalOpen { get; private set; }
    public bool IsProductModalOpen { get; private set; }
    public bool IsSaving { get; private set; }
    public decimal TaxRate { get; set; }

    public decimal Subtotal => OrderDetails.Sum(item => item.UnitPrice * item.Quantity);
    public decimal Tax => decimal.Round(Subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
    public decimal Total => Subtotal + Tax;

    public Task<decimal> LoadTaxRateAsync() => gateway.GetTaxRateAsync();
    public void CloseCreatedInvoice() => CreatedInvoice = null;

    public void OpenCustomerSearch()
    {
        SearchText = string.Empty;
        CustomerFilter = "Todas";
        Page = 1;
        IsCustomerModalOpen = true;
        IsProductModalOpen = false;
    }

    public void OpenProductSearch()
    {
        SearchText = string.Empty;
        ProductFilter = "Todas";
        Page = 1;
        IsProductModalOpen = true;
        IsCustomerModalOpen = false;
    }

    public void CloseModal()
    {
        IsCustomerModalOpen = false;
        IsProductModalOpen = false;
    }

    public async Task SearchAsync()
    {
        Page = 1;
        await LoadPageAsync();
    }

    public async Task ChangePageAsync(int delta)
    {
        int targetPage = Page + delta;
        if (targetPage >= 1 && targetPage <= TotalPages)
        {
            Page = targetPage;
            await LoadPageAsync();
        }
    }

    public async Task GoToPageAsync(int targetPage)
    {
        if (targetPage >= 1 && targetPage <= TotalPages && targetPage != Page)
        {
            Page = targetPage;
            await LoadPageAsync();
        }
    }

    public async Task LoadPageAsync()
    {
        if (IsCustomerModalOpen)
        {
            var result = await gateway.SearchCustomersAsync(SearchText, CustomerFilter, Page, PageSize);
            Customers = result.Items;
            TotalCount = result.TotalCount;
        }
        else if (IsProductModalOpen)
        {
            var result = await gateway.SearchProductsAsync(SearchText, ProductFilter, Page, PageSize);
            Products = result.Items;
            TotalCount = result.TotalCount;
        }
    }

    public void SelectCustomer(CustomerSearchItemDto customer)
    {
        CustomerId = customer.Id;
        CustomerName = customer.Name;
        CustomerBalance = customer.CurrentBalance;
        CloseModal();

        if (customer.CurrentBalance > 0)
        {
            WarningMessage = $"⚠️ Atención: El cliente {customer.Name} ({customer.Id}) registra un adeudo pendiente de ${customer.CurrentBalance.ToString("N2", CultureInfo.InvariantCulture)}. No se permite registrar órdenes mientras mantenga deuda.";
            InformationMessage = string.Empty;
        }
        else
        {
            WarningMessage = string.Empty;
            InformationMessage = string.Empty;
        }
    }

    public void AddProduct(ProductSearchItemDto product)
    {
        var existing = OrderDetails.FirstOrDefault(item => item.ProductId == product.Id);
        if (existing is not null)
        {
            InformationMessage = $"El producto '{product.Name}' ya está incluido en la orden. Puedes modificar la cantidad directamente en la tabla.";
            CloseModal();
            return;
        }

        OrderDetails.Add(new()
        {
            ProductId = product.Id,
            ProductName = product.Name,
            UnitPrice = product.UnitPrice,
            Quantity = 1,
            AvailableStock = product.UnitsInStock
        });
        CloseModal();
        InformationMessage = string.Empty;
        CreatedInvoice = null;
    }

    public void RemoveOrderDetailItem(CreateOrderDetailViewModel item)
    {
        OrderDetails.Remove(item);
    }

    public async Task Send()
    {
        InformationMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(CustomerId))
        {
            InformationMessage = "Debes seleccionar un cliente para la orden.";
            return;
        }

        if (CustomerBalance > 0)
        {
            InformationMessage = $"No se puede procesar la orden: El cliente {CustomerName} ({CustomerId}) tiene un adeudo pendiente de ${CustomerBalance.ToString("N2", CultureInfo.InvariantCulture)}.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ShipAddress))
        {
            InformationMessage = "Debes ingresar la dirección de envío.";
            return;
        }

        if (ShipAddress.Length > 60 ||
            ShipAddress.Any(character => !char.IsLetter(character) && !char.IsDigit(character) && character != ' '))
        {
            InformationMessage = "La dirección debe tener máximo 60 caracteres y solo contener letras, números y espacios.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ShipCity))
        {
            InformationMessage = "Debes ingresar la ciudad de envío.";
            return;
        }

        if (ShipCity.Length < 3 || ShipCity.Length > 15 ||
            ShipCity.Any(character => !char.IsLetter(character) && character != ' '))
        {
            InformationMessage = "La ciudad debe tener entre 3 y 15 caracteres y solo contener letras y espacios.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ShipCountry))
        {
            InformationMessage = "Debes ingresar el país de envío.";
            return;
        }

        if (ShipCountry.Length < 3 || ShipCountry.Length > 15 ||
            ShipCountry.Any(character => !char.IsLetter(character) && character != ' '))
        {
            InformationMessage = "El país debe tener entre 3 y 15 caracteres y solo contener letras y espacios.";
            return;
        }

        if (ShipPostalCode.Length > 10 || ShipPostalCode.Any(character => !char.IsLetterOrDigit(character)))
        {
            InformationMessage = "El código postal debe tener máximo 10 caracteres y solo contener letras y números.";
            return;
        }

        if (OrderDetails.Count == 0)
        {
            InformationMessage = "Debes agregar al menos un producto a la orden.";
            return;
        }

        if (OrderDetails.Any(item => item.Quantity < 1))
        {
            InformationMessage = "La cantidad de cada producto debe ser mayor a cero.";
            return;
        }

        var stockOverflow = OrderDetails.FirstOrDefault(item => item.Quantity > item.AvailableStock);
        if (stockOverflow is not null)
        {
            InformationMessage = $"La cantidad solicitada para '{stockOverflow.ProductName}' ({stockOverflow.Quantity}) supera las existencias disponibles ({stockOverflow.AvailableStock}).";
            return;
        }

        IsSaving = true;
        try
        {
            var orderId = await gateway.CreateOrderAsync((CreateOrderDto)this);
            OrderDetails.Clear();
            CustomerId = string.Empty;
            CustomerName = string.Empty;
            CustomerBalance = 0;
            ShipAddress = string.Empty;
            ShipCity = string.Empty;
            ShipCountry = string.Empty;
            ShipPostalCode = string.Empty;
            WarningMessage = string.Empty;

            try
            {
                CreatedInvoice = await gateway.GetInvoiceAsync(orderId);
                InformationMessage = CreatedInvoice is null
                    ? $"La orden {orderId} se guardó correctamente. Puedes consultarla en la sección de Facturas."
                    : $"¡Orden #{orderId} generada exitosamente! Total: ${CreatedInvoice.Total.ToString("N2", CultureInfo.InvariantCulture)}";
            }
            catch (HttpRequestException)
            {
                InformationMessage = $"La orden {orderId} se guardó correctamente. Puedes consultarla en la sección de Facturas.";
            }
        }
        catch (HttpRequestException exception)
        {
            InformationMessage = exception.Message.Contains("Insufficient stock", StringComparison.OrdinalIgnoreCase)
                ? "Las existencias cambiaron mientras se procesaba la orden. Actualiza los productos e inténtalo nuevamente."
                : (exception.Message.Contains("adeudo pendiente", StringComparison.OrdinalIgnoreCase)
                    ? exception.Message
                    : "No se pudo guardar la orden. Revisa los datos e inténtalo nuevamente.");

            if (exception.Data["Errors"] is IEnumerable<ValidationError> errors)
            {
                ModelValidatorComponentReference?.AddErrors(errors);
                InformationMessage = "Revisa los campos con error e inténtalo nuevamente.";
            }
        }
        catch (Exception)
        {
            InformationMessage = "Ocurrió un error inesperado al procesar la orden. Inténtalo nuevamente.";
        }
        finally
        {
            IsSaving = false;
        }
    }

    public static explicit operator CreateOrderDto(CreateOrderViewModel model) =>
        new(model.CustomerId, model.ShipAddress, model.ShipCity, model.ShipCountry,
            model.ShipPostalCode, model.OrderDetails.Select(item => new CreateOrderDetailDto(
                item.ProductId, item.UnitPrice, (short)Math.Clamp(item.Quantity, 1, short.MaxValue))));
}
