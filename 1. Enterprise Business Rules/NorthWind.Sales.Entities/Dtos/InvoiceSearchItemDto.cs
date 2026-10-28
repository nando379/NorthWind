namespace NorthWind.Sales.Entities.Dtos;

public sealed record InvoiceSearchItemDto(
    int OrderId,
    DateTime OrderDate,
    string CustomerId,
    string CustomerName,
    string? ShipCity = null,
    string? ShipCountry = null,
    decimal Total = 0);
