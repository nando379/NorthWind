namespace NorthWind.Sales.Validators.Entities.CreateOrder;

internal class CreateOrderDtoValidator :
    AbstractModelValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator(
        IValidationService<CreateOrderDto> validationService,
        IModelValidator<CreateOrderDetailDto> detailValidator) :
        base(validationService)
    {
        AddRuleFor(c => c.CustomerId)
            .NotEmpty(CreateOrderMessages.CustomerIdRequired)
            .Length(5, CreateOrderMessages.CustomerIdRequiredLength);

        AddRuleFor(c => c.ShipAddress)
            .NotEmpty(CreateOrderMessages.ShipAddressRequired)
            .MaximumLength(60, CreateOrderMessages.ShipAddressMaximumLength)
            .Must(address => string.IsNullOrWhiteSpace(address) || address.All(IsAddressCharacterAllowed),
                CreateOrderMessages.ShipAddressAllowedCharacters);

        AddRuleFor(c => c.ShipCity)
            .NotEmpty(CreateOrderMessages.ShipCityRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCityMinimumLength)
            .MaximumLength(15, CreateOrderMessages.ShipCityMaximumLength)
            .Must(city => string.IsNullOrWhiteSpace(city) || city.All(IsLetterOrSpace),
                CreateOrderMessages.ShipCityLettersOnly);

        AddRuleFor(c => c.ShipCountry)
            .NotEmpty(CreateOrderMessages.ShipCountryRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCountryMinimumLength)
            .MaximumLength(15, CreateOrderMessages.ShipCountryMaximumLength)
            .Must(country => string.IsNullOrWhiteSpace(country) || country.All(IsLetterOrSpace),
                CreateOrderMessages.ShipCountryLettersOnly);

        AddRuleFor(c => c.ShipPostalCode)
            .MaximumLength(10, CreateOrderMessages.ShipPostalCodeMaximumLength)
            .Must(postalCode => string.IsNullOrWhiteSpace(postalCode) || postalCode.All(char.IsLetterOrDigit),
                CreateOrderMessages.ShipPostalCodeAllowedCharacters);

        AddRuleFor(c => c.OrderDetails)
            .NotNull(CreateOrderMessages.OrderDetailsRequired)
            .NotEmpty(CreateOrderMessages.OrderDetailsNotEmpty);

        AddRuleForEach(c => c.OrderDetails)
            .SetValidator(detailValidator);
    }

    private static bool IsAddressCharacterAllowed(char character) =>
        char.IsLetter(character) || character == ' ';

    private static bool IsLetterOrSpace(char character) =>
        char.IsLetter(character) || character == ' ';
}
