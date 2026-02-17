using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Events.WebApi.Authentication;

public sealed class AuthenticationEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly TokenRequest ValidTokenRequest = new()
    {
        Username = "user",
        Password = "password"
    };

    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task PostToken_ReturnsResponseWithBadRequestStatusCode_WhenRequestResourceIsInvalid()
    {
        TokenRequest invalidTokenRequest = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.GenerateTokenRoute, invalidTokenRequest, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostToken_ReturnsValidationErrorsInResponseBody_WhenRequestResourceIsInvalid()
    {
        TokenRequest invalidTokenRequest = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.GenerateTokenRoute, invalidTokenRequest, TestContext.Current.CancellationToken);

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