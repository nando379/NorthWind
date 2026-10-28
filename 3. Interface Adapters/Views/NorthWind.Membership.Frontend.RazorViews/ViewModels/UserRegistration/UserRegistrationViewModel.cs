namespace NorthWind.Membership.Frontend.RazorViews.ViewModels.UserRegistration;

public class UserRegistrationViewModel(
    MembershipGateway gateway,
    IModelValidatorHub<UserRegistrationViewModel> validator)
{
    public IModelValidatorHub<UserRegistrationViewModel> Validator => validator;
    public ModelValidator<UserRegistrationViewModel> ModelValidatorComponentReference { get; set; }
    public string InformationMessage { get; private set; }

    #region Datos de entrada
    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => _firstName = System.Text.RegularExpressions.Regex.Replace(value ?? string.Empty, @"[^a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]", "");
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set => _lastName = System.Text.RegularExpressions.Regex.Replace(value ?? string.Empty, @"[^a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]", "");
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = (value ?? string.Empty).Replace(" ", "");
    }

    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
    #endregion

    public async Task Register()
    {
        try
        {
            InformationMessage = "";
            await gateway.RegisterAsync((UserRegistrationDto)this);
            InformationMessage = UserRegistrationMessages.RegisteredUserMessage;
        }
        catch (HttpRequestException ex)
        {
            if (ex.Data.Contains("Errors"))
            {
                IEnumerable<ValidationError> Errors =
                    ex.Data["Errors"] as IEnumerable<ValidationError>;
                ModelValidatorComponentReference.AddErrors(Errors);
            }
            else
            {
                throw;
            }
        }
    }

    public static explicit operator UserRegistrationDto(
        UserRegistrationViewModel model) =>
        new UserRegistrationDto(
            model.Email, model.Password,
            model.FirstName, model.LastName);
}
