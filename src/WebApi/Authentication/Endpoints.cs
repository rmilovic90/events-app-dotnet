using System.Net.Mime;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using RootEndpoints = Events.WebApi.Endpoints;

namespace Events.WebApi.Authentication;

internal static class Endpoints
{
    private const string BaseRoute = $"{RootEndpoints.ApiBaseRoute}/authentication";
    internal const string GenerateTokenRoute = $"{BaseRoute}/token";

    private static IResult GenerateToken(TokenRequest tokenRequest, IConfiguration config)
    {
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(config["JWT:Key"]!));

        string token = new JsonWebTokenHandler()
            .CreateToken
            (
                new SecurityTokenDescriptor()
                {
                    Issuer = config["JWT:Issuer"],
                    Audience = config["JWT:Audience"],
                    Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, tokenRequest.Username)]),
                    Expires = DateTime.Now.AddSeconds(Token.DefaultExpirationInSeconds),
                    SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                }
            );

        return TypedResults.Ok(Token.BearerWithDefaultExpiration(token));
    }

    public static IEndpointRouteBuilder RegisterAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(GenerateTokenRoute, GenerateToken)
            .WithDescription($"Generate authentication token of type {JwtBearerDefaults.AuthenticationScheme}.")
            .WithTags("Authentication")
            .Accepts<TokenRequest>(MediaTypeNames.Application.Json)
            .Produces<Token>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson);

        return endpoints;
    }
}