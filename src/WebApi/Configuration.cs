using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace Events.WebApi;

internal static class Configuration
{
    internal static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi
        (
            options =>
            {
                options.AddDocumentTransformer
                (
                    static (document, context, cancellationToken) =>
                    {
                        document.Components ??= new OpenApiComponents();
                        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                        document.Security ??= [];

                        document.Components.SecuritySchemes.Add
                        (
                            JwtBearerDefaults.AuthenticationScheme,
                            new OpenApiSecurityScheme
                            {
                                Scheme = JwtBearerDefaults.AuthenticationScheme,
                                Type = SecuritySchemeType.Http,
                                In = ParameterLocation.Header
                            }
                        );

                        document.Security.Add(new()
                        {
                            {
                                new OpenApiSecuritySchemeReference
                                (
                                    JwtBearerDefaults.AuthenticationScheme, document
                                ),
                                []
                            }
                        });

                        return Task.CompletedTask;
                    }
                );

                options.AddSchemaTransformer
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
                );
            }
        );

        return services;
    }

    internal static IServiceCollection ConfigureSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer
            (
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = configuration["JWT:Issuer"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = configuration["JWT:Audience"],
                        ValidIssuer = configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!))
                    };
                }
            );
        services.AddAuthorization();

        return services;
    }

    internal static WebApplication UseOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Events API v1"));

        return app;
    }

    internal static IApplicationBuilder UseSecurity(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseAuthentication();
        applicationBuilder.UseAuthorization();

        return applicationBuilder;
    }
}