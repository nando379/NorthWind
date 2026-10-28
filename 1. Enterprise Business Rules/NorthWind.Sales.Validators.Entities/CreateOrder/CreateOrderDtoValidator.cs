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
            .MaximumLength(60, CreateOrderMessages.ShipAddressMaximumLength);

        AddRuleFor(c => c.ShipCity)
            .NotEmpty(CreateOrderMessages.ShipCityRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCityMinimumLength)
            .MaximumLength(15, CreateOrderMessages.ShipCityMaximumLength)
            .Must(city => string.IsNullOrWhiteSpace(city) || city.All(character =>
                    char.IsLetter(character) || char.IsWhiteSpace(character)),
                CreateOrderMessages.ShipCityLettersOnly);

        AddRuleFor(c => c.ShipCountry)
            .NotEmpty(CreateOrderMessages.ShipCountryRequired)
            .MinimumLength(3, CreateOrderMessages.ShipCountryMinimumLength)
            .MaximumLength(15, CreateOrderMessages.ShipCountryMaximumLength)
            .Must(country => string.IsNullOrWhiteSpace(country) || country.All(character =>
                    char.IsLetter(character) || char.IsWhiteSpace(character)),
                CreateOrderMessages.ShipCountryLettersOnly);

        AddRuleFor(c => c.ShipPostalCode)
            .MaximumLength(10, CreateOrderMessages.ShipPostalCodeMaximumLength);

        AddRuleFor(c => c.OrderDetails)
            .NotNull(CreateOrderMessages.OrderDetailsRequired)
            .NotEmpty(CreateOrderMessages.OrderDetailsNotEmpty);

        AddRuleForEach(c => c.OrderDetails)
            .SetValidator(detailValidator);
    }
}
