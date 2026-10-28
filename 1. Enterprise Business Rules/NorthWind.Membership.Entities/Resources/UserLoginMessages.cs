namespace NorthWind.Membership.Entities.Resources;

public class UserLoginMessages
{
    public static string RequiredEmailErrorMessage => "El correo es requerido.";
    public static string InvalidEmailErrorMessage => "El formato de correo no es válido.";
    public static string EmailCannotContainSpacesErrorMessage => "El correo electrónico no puede contener espacios.";
    public static string RequiredPasswordErrorMessage => "La contraseña es requerida.";
}
