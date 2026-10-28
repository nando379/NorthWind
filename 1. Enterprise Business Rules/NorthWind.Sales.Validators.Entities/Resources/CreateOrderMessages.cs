namespace NorthWind.Sales.Validators.Entities.Resources;

public class CreateOrderMessages
{
    public static string ProductIdGreaterThanZero => "El identificador del producto debe ser mayor que cero.";
    public static string QuantityGreaterThanZero => "La cantidad debe ser mayor que cero y no puede ser negativa.";
    public static string QuantityMaxValue => "La cantidad no debe superar 32767 unidades.";
    public static string UnitPriceGreaterThanZero => "El precio debe ser mayor que cero y no puede ser negativo.";
    public static string CustomerIdRequired => "Debe proporcionar el identificador del cliente.";
    public static string CustomerIdRequiredLength => "La longitud del identificador debe ser 5.";
    public static string ShipAddressRequired => "Debe proporcionar la dirección de envío.";
    public static string ShipAddressMaximumLength => "La longitud máxima de la dirección es 60.";
    public static string ShipCityRequired => "Debe proporcionar la ciudad de envío.";
    public static string ShipCityMinimumLength => "Debe especificar al menos 3 caracteres del nombre de la ciudad.";
    public static string ShipCityMaximumLength => "La longitud máxima de la ciudad es 15.";
    public static string ShipCityNoNumbers => "El nombre de la ciudad no debe contener números.";
    public static string ShipCountryRequired => "Debe proporcionar el país de envío.";
    public static string ShipCountryMinimumLength => "Debe especificar al menos 3 caracteres del nombre del País.";
    public static string ShipCountryMaximumLength => "La longitud máxima del país es 15.";
    public static string ShipCountryNoNumbers => "El nombre del país no debe contener números.";
    public static string ShipPostalCodeMaximumLength => "La longitud máxima del código postal es 10.";
    public static string OrderDetailsRequired => "Debe especificar los productos de la orden.";
    public static string OrderDetailsNotEmpty => "Debe especificar al menos un producto de la orden.";
}
