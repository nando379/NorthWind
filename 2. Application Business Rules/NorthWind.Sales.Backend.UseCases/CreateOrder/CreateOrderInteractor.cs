namespace NorthWind.Sales.Backend.UseCases.CreateOrder;

internal class CreateOrderInteractor(
    ICreateOrderOutputPort outputPort,
    ICommandsRepository repository,
    IModelValidatorHub<CreateOrderDto> modelValidatorHub,
    IDomainEventHub<SpecialOrderCreatedEvent> domainEventHub,
    IDomainLogger domainLogger,
    IDomainTransaction domainTransaction,
    IUserService userService,
    ITaxRateProvider taxRateProvider) :
    ICreateOrderInputPort
{
    public async Task Handle(CreateOrderDto orderDto)
    {
        GuardUser.AgainstUnauthenticated(userService);

        await GuardModel.AgainstNotValid(modelValidatorHub, orderDto);

        await domainLogger.LogInformation(new DomainLog(
            CreateOrderMessages.StartingPurchaseOrderCreation,
            userService.UserName));

        OrderAggregate Order = OrderAggregate.From(orderDto, taxRateProvider.CurrentRate);

        try
        {
            // Iniciar la transacción
            domainTransaction.BeginTransaction();

            await repository.CreateOrder(Order);
            await repository.SaveChanges();
            domainTransaction.CommitTransaction();
        }
        catch
        {
            // Cancelar la transacción
            domainTransaction.RollbackTransaction();

            string Information = string.Format(
                CreateOrderMessages.OrderCreationCancelledTemplate,
                Order.Id);

            await domainLogger.LogInformation(
                new DomainLog(Information, userService.UserName));
            throw;
        }

        await domainLogger.LogInformation(new DomainLog(string.Format(
            CreateOrderMessages.PurchaseOrderCreatedTemplate, Order.Id),
            userService.UserName));
        await outputPort.Handle(Order);

        if (new SpecialOrderSpecification().IsSatisfiedBy(Order))
        {
            try
            {
                await domainEventHub.Raise(new SpecialOrderCreatedEvent(
                    Order.Id, Order.OrderDetails.Count));
            }
            catch (Exception)
            {
                await domainLogger.LogInformation(new DomainLog(
                    $"Order {Order.Id} was saved, but its notification could not be sent.",
                    userService.UserName));
            }
        }
    }
}
