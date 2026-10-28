namespace NorthWind.Sales.Backend.IoC;

internal sealed class ConfigurationTaxRateProvider(IOptions<SalesTaxOptions> options) : ITaxRateProvider
{
    public decimal CurrentRate => options.Value.TaxRate;
}
