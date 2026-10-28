namespace NorthWind.Exceptions.Entities.Extensions;

internal static class HttpContextExtensions
{
    public static async ValueTask WriteProblemDetailsAsync(
        this HttpContext context, ProblemDetails details)
    {
        // ProblemDetails tiene su propio content-type.
        context.Response.ContentType = "application/problem+json";
        // Código de respuesta HTTP a devolver.
        context.Response.StatusCode = details.Status.Value;
        // Serializar la respuesta.
        var Stream = context.Response.Body;
        await JsonSerializer.SerializeAsync(Stream, details);
    }
}
