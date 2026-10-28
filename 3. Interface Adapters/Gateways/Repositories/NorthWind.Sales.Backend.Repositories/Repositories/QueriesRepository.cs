namespace NorthWind.Sales.Backend.Repositories.Repositories;

internal class QueriesRepository(INorthWindSalesQueriesDataContext context) :
    IQueriesRepository
{
    public async Task<decimal?> GetCustomerCurrentBalance(
        string customerId)
    {
        var Queryable = context.Customers
            .Where(c => c.Id == customerId)
            .Select(c => new { c.CurrentBalance });
        var Result = await context.FirstOrDefaultAync(Queryable);
        return Result?.CurrentBalance;
    }

    public async Task<IEnumerable<ProductUnitsInStock>>
        GetProductsUnitsInStock(IEnumerable<int> productIds)
    {
        var Queryable = context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new ProductUnitsInStock(
                p.Id, p.UnitsInStock));
        return await context.ToListAsync(Queryable);
    }

    public async Task<SearchPageDto<CustomerSearchItemDto>> SearchCustomers(string? search, string? field, int page, int pageSize)
    {
        var query = context.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            if (string.Equals(field, "ID", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(customer => customer.Id.Contains(term));
            }
            else if (string.Equals(field, "Nombre", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(customer => customer.Name.Contains(term));
            }
            else
            {
                query = query.Where(customer => customer.Id.Contains(term) || customer.Name.Contains(term));
            }
        }
        var total = await context.CountAsync(query);
        var items = await context.ToListAsync(query.OrderBy(customer => customer.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(customer => new CustomerSearchItemDto(customer.Id, customer.Name, customer.CurrentBalance)));
        return new(items.ToList(), page, pageSize, total);
    }

    public async Task<SearchPageDto<ProductSearchItemDto>> SearchProducts(string? search, string? field, int page, int pageSize)
    {
        var query = context.Products.Where(product => product.UnitsInStock > 0);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            if (string.Equals(field, "ID", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(product => product.Id.ToString().Contains(term));
            }
            else if (string.Equals(field, "Nombre", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(product => product.Name.Contains(term));
            }
            else if (string.Equals(field, "Stock", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(product => product.UnitsInStock.ToString().Contains(term));
            }
            else if (string.Equals(field, "Precio", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(product => product.UnitPrice.ToString().Contains(term));
            }
            else
            {
                query = query.Where(product => product.Name.Contains(term) || product.Id.ToString().Contains(term));
            }
        }
        var total = await context.CountAsync(query);
        var items = await context.ToListAsync(query.OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(product => new ProductSearchItemDto(product.Id, product.Name, product.UnitPrice, product.UnitsInStock)));
        return new(items.ToList(), page, pageSize, total);
    }

    public async Task<SearchPageDto<InvoiceSearchItemDto>> SearchInvoices(string? search, string? field, int page, int pageSize)
    {
        var query = from order in context.Orders
                    join customer in context.Customers on order.CustomerId equals customer.Id
                    select new
                    {
                        order.Id,
                        order.OrderDate,
                        CustomerId = customer.Id,
                        CustomerName = customer.Name,
                        order.ShipCity,
                        order.ShipCountry,
                        order.TaxRate
                    };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            if (string.Equals(field, "Número de factura", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, "Numero de factura", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, "Id", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(item => item.Id.ToString().Contains(term));
            }
            else if (string.Equals(field, "Cliente / nombre", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(field, "Cliente", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(field, "Nombre", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(item => item.CustomerName.Contains(term) || item.CustomerId.Contains(term));
            }
            else if (string.Equals(field, "Fecha", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(item => item.OrderDate.ToString().Contains(term));
            }
            else if (string.Equals(field, "Ciudad", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(item => item.ShipCity.Contains(term) || item.ShipCountry.Contains(term));
            }
            else
            {
                query = query.Where(item => item.CustomerName.Contains(term)
                    || item.CustomerId.Contains(term)
                    || item.Id.ToString().Contains(term)
                    || item.ShipCity.Contains(term)
                    || item.ShipCountry.Contains(term));
            }
        }

        var total = await context.CountAsync(query);
        var pagedOrders = (await context.ToListAsync(query.OrderByDescending(item => item.OrderDate)
            .Skip((page - 1) * pageSize).Take(pageSize))).ToList();

        var orderIds = pagedOrders.Select(o => o.Id).ToList();
        var detailsQuery = context.OrderDetails.Where(d => orderIds.Contains(d.OrderId));
        var details = (await context.ToListAsync(detailsQuery)).ToList();
        var detailsByOrder = details.GroupBy(d => d.OrderId).ToDictionary(g => g.Key, g => g.ToList());

        var items = new List<InvoiceSearchItemDto>();
        foreach (var order in pagedOrders)
        {
            decimal orderTotal = 0;
            if (detailsByOrder.TryGetValue(order.Id, out var ordDetails))
            {
                var subtotal = ordDetails.Sum(d => d.UnitPrice * d.Quantity);
                var tax = decimal.Round(subtotal * order.TaxRate, 2, MidpointRounding.AwayFromZero);
                orderTotal = subtotal + tax;
            }
            items.Add(new InvoiceSearchItemDto(
                order.Id,
                order.OrderDate,
                order.CustomerId,
                order.CustomerName,
                order.ShipCity,
                order.ShipCountry,
                orderTotal));
        }

        return new(items, page, pageSize, total);
    }

    public async Task<InvoiceDto?> GetInvoice(int orderId)
    {
        var order = await context.FirstOrDefaultAync(context.Orders.Where(item => item.Id == orderId));
        if (order is null) return null;
        var customer = await context.FirstOrDefaultAync(context.Customers.Where(item => item.Id == order.CustomerId));
        var details = await context.ToListAsync(
            from detail in context.OrderDetails.Where(item => item.OrderId == orderId)
            join product in context.Products on detail.ProductId equals product.Id
            select new InvoiceDetailDto(detail.ProductId, product.Name, detail.UnitPrice,
                detail.Quantity, detail.UnitPrice * detail.Quantity));
        var subtotal = details.Sum(item => item.LineSubtotal);
        var tax = decimal.Round(subtotal * order.TaxRate, 2, MidpointRounding.AwayFromZero);
        return new(order.Id, order.OrderDate, order.CustomerId, customer?.Name ?? string.Empty,
            order.ShipAddress, order.ShipCity, order.ShipCountry, order.ShipPostalCode,
            details.ToList(), subtotal, order.TaxRate, tax, subtotal + tax);
    }
}
