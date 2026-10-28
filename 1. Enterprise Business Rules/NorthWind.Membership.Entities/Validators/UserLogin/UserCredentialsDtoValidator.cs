namespace NorthWind.Membership.Entities.Validators.UserLogin;

internal class UserCredentialsDtoValidator :
    AbstractModelValidator<UserCredentialsDto>
{
    public UserCredentialsDtoValidator(
        IValidationService<UserCredentialsDto> validationService) :
        base(validationService)
    {
        AddRuleFor(u => u.Email)
            .NotEmpty(UserLoginMessages.RequiredEmailErrorMessage)
            .EmailAddress(UserLoginMessages.InvalidEmailErrorMessage);
        AddRuleFor(u => u.Password)
            .NotEmpty(UserLoginMessages.RequiredPasswordErrorMessage);
    }
}
