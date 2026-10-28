namespace NorthWind.Sales.Entities.Dtos;

public sealed record CustomerSearchItemDto(string Id, string Name, decimal CurrentBalance);
public sealed record ProductSearchItemDto(int Id, string Name, decimal UnitPrice, short UnitsInStock);
public sealed record InvoiceDetailDto(int ProductId, string ProductName, decimal UnitPrice, short Quantity, decimal LineSubtotal);
public sealed record InvoiceDto(int OrderId, DateTime OrderDate, string CustomerId, string CustomerName, string ShipAddress, string ShipCity, string ShipCountry, string ShipPostalCode, IReadOnlyList<InvoiceDetailDto> Details, decimal Subtotal, decimal TaxRate, decimal Tax, decimal Total);
