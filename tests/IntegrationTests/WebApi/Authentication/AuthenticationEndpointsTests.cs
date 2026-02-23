using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

using static Events.WebApi.Authentication.TokenRequestResourceBuilder;

namespace Events.WebApi.Authentication;

public sealed class AuthenticationEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly TokenRequest InvalidTokenRequest = new();
    private static readonly TokenRequest ValidTokenRequest = ATokenRequestResource.Build();

    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task PostToken_ReturnsResponseWithBadRequestStatusCode_WhenRequestResourceIsInvalid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.GenerateTokenRoute, InvalidTokenRequest, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostToken_ReturnsValidationErrorsInResponseBody_WhenRequestResourceIsInvalid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.GenerateTokenRoute, InvalidTokenRequest, TestContext.Current.CancellationToken);

        ValidationProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody?.Errors);
        Assert.Multiple
        (
            () => Assert.Contains(nameof(TokenRequest.Username), responseBody.Errors),
            () => Assert.Contains(nameof(TokenRequest.Password), responseBody.Errors)
        );
    }

    [Fact]
    public async Task PostEvent_ReturnsResponseWithOkStatusCode_WhenRequestResourceIsValid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.GenerateTokenRoute, ValidTokenRequest, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostEvent_ReturnsResponseWithTokenBody_WhenRequestResourceIsValid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.GenerateTokenRoute, ValidTokenRequest, TestContext.Current.CancellationToken);

        Token? token = await response.Content.ReadFromJsonAsync<Token>(TestContext.Current.CancellationToken);

        Assert.NotNull(token);
        Assert.Multiple
        (
            () => Assert.NotEmpty(token.AccessToken),
            () => Assert.Equal(JwtBearerDefaults.AuthenticationScheme, token.TokenType),
            () => Assert.Equal(Token.DefaultExpirationInSeconds, token.ExpiresIn)
        );
    }
}