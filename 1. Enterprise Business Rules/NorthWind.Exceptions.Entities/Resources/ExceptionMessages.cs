namespace NorthWind.Exceptions.Entities.Resources;

public class ExceptionMessages
{
    public static string ValidationExceptionTitle => "Error en los datos de entrada.";
    public static string ValidationExceptionDetail => "Se encontraron uno o más errores de validación de datos.";
    public static string UpdateExceptionTitle => "Error de actualización.";
    public static string UpdateExceptionDetail => "Se encontraron uno o más errores de actualización de datos. Consulte al administrador.";
    public static string UnhandledExceptionTitle => "El servicio no se encuentra disponible.";
    public static string UnhandledExceptionDetail => "Consulte al administrador.";
    public static string UnauthorizedAccessExceptionTitle => "Acceso no autorizado.";
    public static string UnauthorizedAccessExceptionDetail => "El acceso al recurso solicitado está restringido a usuarios autorizados.";
}
