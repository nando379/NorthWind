namespace NorthWind.Sales.Entities.ValueObjects;

public class Endpoints
{
    public const string CreateOrder = $"/{nameof(CreateOrder)}";
    public const string SearchCustomers = "/customers";
    public const string SearchProducts = "/products";
    public const string SearchInvoices = "/invoices";
    public const string GetInvoice = "/invoices/{orderId:int}";
    public const string GetTaxRate = "/sales/tax-rate";
}
