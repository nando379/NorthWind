namespace NorthWind.Membership.Entities.Validators.UserRegistration;

internal class UserRegistrationDtoValidator :
    AbstractModelValidator<UserRegistrationDto>
{
    public UserRegistrationDtoValidator(
        IValidationService<UserRegistrationDto> validationService) :
        base(validationService)
    {
        AddRuleFor(u => u.FirstName)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredFirstNameErrorMessage)
            .MaximumLength(50, UserRegistrationMessages.FirstNameTooLongErrorMessage)
            .Must(name => !string.IsNullOrWhiteSpace(name) && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)),
                UserRegistrationMessages.InvalidFirstNameErrorMessage);
        AddRuleFor(u => u.LastName)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredLastNameErrorMessage)
            .MaximumLength(50, UserRegistrationMessages.LastNameTooLongErrorMessage)
            .Must(name => !string.IsNullOrWhiteSpace(name) && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)),
                UserRegistrationMessages.InvalidLastNameErrorMessage);
        AddRuleFor(u => u.Email)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredEmailErrorMessage)
            .Must(email => !string.IsNullOrEmpty(email) && !email.Any(char.IsWhiteSpace),
                UserRegistrationMessages.EmailCannotContainSpacesErrorMessage)
            .EmailAddress(UserRegistrationMessages.InvalidEmailErrorMessage);
        AddRuleFor(u => u.Password)
            .StopOnFirstFailure()
            .NotEmpty(UserRegistrationMessages.RequiredPasswordErrorMessage)
            .MinimumLength(6,
                UserRegistrationMessages.PasswordTooShortErrorMessage)
            .Must(p => p.Any(c => char.IsLower(c)),
                UserRegistrationMessages.PasswordRequiresLowerErrorMessage)
            .Must(p => p.Any(c => char.IsUpper(c)),
                UserRegistrationMessages.PasswordRequiresUpperErrorMessage)
            .Must(p => p.Any(c => char.IsDigit(c)),
                UserRegistrationMessages.PasswordRequiresDigitErrorMessage)
            .Must(p => p.Any(c => !char.IsLetterOrDigit(c)),
                UserRegistrationMessages.PasswordRequiresNonAlphanumericErrorMessage);
    }
}
