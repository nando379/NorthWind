namespace NorthWind.Sales.Backend.UseCases.CreateOrder;

internal class CreateOrderProductValidator(
    IQueriesRepository repository) : IModelValidator<CreateOrderDto>
{
    readonly List<ValidationError> ErrorsField = [];
    public IEnumerable<ValidationError> Errors => ErrorsField;

    public ValidationConstraint Constraint =>
        ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

    public async Task<bool> Validate(CreateOrderDto model)
    {
        // Obtener los Ids de productos con las cantidades requeridas.
        // Si hay productos repetidos, se suman las cantidades.
        IEnumerable<ProductUnitsInStock> RequiredQuantities =
            model.OrderDetails
                .GroupBy(d => d.ProductId)
                .Select(d => new ProductUnitsInStock(
                    d.Key, (short)d.Sum(d => d.Quantity)));

        IEnumerable<int> ProductIds =
            RequiredQuantities.Select(d => d.ProductId);

        // Obtener las existencias en almacén.
        IEnumerable<ProductUnitsInStock> InStockQuantities =
            await repository.GetProductsUnitsInStock(ProductIds);

        // Realizar una operación Left Join (Uno a muchos) para obtener una colección de elementos:
        // {int ProductId, short Required, short? InStock}.
        var RequiredVsInStock = RequiredQuantities
            .GroupJoin(InStockQuantities,
                required => required.ProductId, inStock => inStock.ProductId,
                (oneRequired, manyInStock) => new { oneRequired, manyInStock })
            .SelectMany(groupResult =>
                groupResult.manyInStock.DefaultIfEmpty(), // null si no existe el Id
                (groupResult, singleInStock) =>
                    new
                    {
                        groupResult.oneRequired.ProductId,
                        Required = groupResult.oneRequired.UnitsInStock,
                        InStock = singleInStock?.UnitsInStock
                    });

        // Verificar que cada producto exista y que la cantidad en existencia sea mayor o igual a la cantidad requerida.
        foreach (var Item in RequiredVsInStock)
        {
            if (!Item.InStock.HasValue)
            {
                string PropertyName =
                    GetPropertyNameWithIndex(model, Item.ProductId,
                        nameof(CreateOrderDetailDto.ProductId));

                ErrorsField.Add(new ValidationError(PropertyName,
                    string.Format(
                        CreateOrderMessages.ProductIdNotFoundErrorTemplate,
                        Item.ProductId)));
            }
            else
            {
                if (Item.InStock < Item.Required)
                {
                    string PropertyName =
                        GetPropertyNameWithIndex(model, Item.ProductId,
                            nameof(CreateOrderDetailDto.Quantity));

                    ErrorsField.Add(new ValidationError(PropertyName,
                        string.Format(CreateOrderMessages
                            .UnitsInStockLessThanQuantityErrorTemplate,
                            Item.Required, Item.InStock, Item.ProductId)));
                }
            }
        }

        return !ErrorsField.Any();
    }

    string GetPropertyNameWithIndex(CreateOrderDto model,
        int productId, string propertyName)
    {
        var OrderDetail = model.OrderDetails
            .First(i => i.ProductId == productId);

        var OrderDetaiIndex = model.OrderDetails.ToList()
            .IndexOf(OrderDetail);

        return string.Format("{0}[{1}].{2}",
            nameof(model.OrderDetails),
            OrderDetaiIndex,
            propertyName);
    }
}
