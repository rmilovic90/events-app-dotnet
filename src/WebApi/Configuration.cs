using Microsoft.OpenApi;

namespace Events.WebApi;

internal static class Configuration
{
    internal static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi
        (
            options => options.AddSchemaTransformer
            (
                static (schema, context, cancellationToken) =>
                {
                    if (string.Equals(schema.Format, nameof(Int32), StringComparison.OrdinalIgnoreCase) && schema.Type.HasValue)
                    {
                        schema.Format = null;
                        schema.Pattern = null;
                        schema.Type = JsonSchemaType.Integer;
                    }

                    return Task.CompletedTask;
                }
            )
        );

        return services;
    }

    internal static WebApplication UseOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Events API v1"));

        return app;
    }
}