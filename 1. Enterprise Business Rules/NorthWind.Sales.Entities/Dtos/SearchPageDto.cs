namespace NorthWind.Sales.Entities.Dtos;

public sealed record SearchPageDto<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
