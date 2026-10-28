namespace NorthWind.Sales.Backend.UseCases.Resources;

public class CreateOrderMessages
{
    public static string CustomerIdNotFoundError =>
        "El identificador de cliente proporcionado no existe.";

    public static string CustomerWithBalanceErrorTemplate =>
        "El cliente {0} tiene un adeudo pendiente de {1}.";

    public static string ProductIdNotFoundErrorTemplate =>
        "El producto {0} no existe.";

    public static string UnitsInStockLessThanQuantityErrorTemplate =>
        "Cantidad {0} de {1} no suficiente para producto {2}.";

    public static string SendEmailSubject =>
        "Orden especial creada";

    public static string SendEmailBodyTemplate =>
        "Se ha creado la orden {0} con {1} productos.";

    public static string StartingPurchaseOrderCreation =>
        "Inicio de creación de orden de compra";

    public static string PurchaseOrderCreatedTemplate =>
        "Orden de compra {0} creada.";

    public static string OrderCreationCancelledTemplate =>
        "Creación de orden {0} cancelada.";
}
