namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerGenBearer(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Agregar definición de seguridad.
            options.AddSecurityDefinition("BearerToken",
                new OpenApiSecurityScheme
                {
                    // Nombre del encabezado HTTP que se agregará en la petición.
                    Name = "Authorization",
                    Description = "Proporciona el valor del token.",
                    // Ubicación del Token
                    In = ParameterLocation.Header,
                    // Tipo del esquema de seguridad a utilizar.
                    Type = SecuritySchemeType.Http,
                    // Nombre del esquema de autenticación.
                    Scheme = "Bearer"
                });

            // Para agregar el encabezado de autorización en las peticiones de endpoints.
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "BearerToken"
                            }
                        },
                        new string[] { }
                    }
                });
        });
        return services;
    }
}
