namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection AddMembershipServices(
        this IServiceCollection services,
        Action<HttpClient> configureHttpClient)
    {
        services.AddAuthorizationCore();
        services.AddScoped<ITokenStorage, TokenLocalStorage>();
        services.AddScoped<JWTAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(provider =>
            provider.GetService<JWTAuthenticationStateProvider>());

        services.AddExceptionDelegatingHandler();
        services.AddHttpClient<MembershipGateway>(configureHttpClient)
            .AddHttpMessageHandler<ExceptionDelegatingHandler>();

        services.AddScoped<UserRegistrationViewModel>();
        services.AddMembershipValidators();
        services.AddModelValidator<UserRegistrationViewModel,
            UserRegistrationViewModelDtoValidator>();
        services.AddModelValidator<UserRegistrationViewModel,
            UserRegistrationViewModelValidator>();

        services.AddScoped<UserLoginViewModel>();
        services.AddModelValidator<UserLoginViewModel,
            UserLoginViewModelDtoValidator>();

        return services;
    }

    public static IHttpClientBuilder AddMembershipBearerTokenHandler(
        this IHttpClientBuilder builder)
    {
        builder.Services.TryAddTransient<MembershipBearerTokenHandler>();
        builder.AddHttpMessageHandler<MembershipBearerTokenHandler>();
        return builder;
    }
}
