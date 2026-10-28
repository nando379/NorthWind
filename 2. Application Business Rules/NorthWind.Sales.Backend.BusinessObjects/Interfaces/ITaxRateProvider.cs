namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces;

public interface ITaxRateProvider
{
    decimal CurrentRate { get; }
}
