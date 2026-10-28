namespace NorthWind.Sales.Backend.BusinessObjects.Options;

public sealed class SalesTaxOptions
{
    public const string SectionKey = "Sales";
    public decimal TaxRate { get; set; }
}
