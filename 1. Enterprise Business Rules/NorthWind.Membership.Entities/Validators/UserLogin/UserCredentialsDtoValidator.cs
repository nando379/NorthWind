namespace NorthWind.Membership.Entities.Validators.UserLogin;

internal class UserCredentialsDtoValidator :
    AbstractModelValidator<UserCredentialsDto>
{
    public UserCredentialsDtoValidator(
        IValidationService<UserCredentialsDto> validationService) :
        base(validationService)
    {
        AddRuleFor(u => u.Email)
            .StopOnFirstFailure()
            .NotEmpty(UserLoginMessages.RequiredEmailErrorMessage)
            .Must(email => !string.IsNullOrEmpty(email) && !email.Any(char.IsWhiteSpace),
                UserLoginMessages.EmailCannotContainSpacesErrorMessage)
            .EmailAddress(UserLoginMessages.InvalidEmailErrorMessage);
        AddRuleFor(u => u.Password)
            .NotEmpty(UserLoginMessages.RequiredPasswordErrorMessage);
    }
}
