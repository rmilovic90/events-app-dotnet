using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Events.WebApi.Events;

public sealed class EventEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly DateTime UtcTomorrow = DateTime.UtcNow.Date.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private static readonly Event ValidEvent = new()
    {
        Name = "Test",
        Description = "The test event.",
        Location = "Novi Sad, Serbia",
        StartTime = new DateTimeOffset(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 14, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)),
        EndTime = new DateTimeOffset(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 15, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow))
    };

    private readonly HttpClient _httpClient = factory.WithWebHostBuilder
    (
        builder => builder.ConfigureTestServices
        (
            services => services.AddAuthentication(TestAuthHandler.TestAuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>
                (
                    TestAuthHandler.TestAuthenticationScheme,
                    options => { }
                )
        )
    )
    .CreateClient();

    [Fact]
    public async Task PostEvent_ReturnsResponseWithBadRequestStatusCode_WhenResourceIsInvalid()
    {
        Event invalidEvent = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, invalidEvent, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostEvent_ReturnsValidationErrorsInResponseBody_WhenResourceIsInvalid()
    {
        DateTime now = DateTime.UtcNow;
        Event invalidEvent = new()
        {
            StartTime = now,
            EndTime = now
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, invalidEvent, TestContext.Current.CancellationToken);

        ValidationProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody?.Errors);
        Assert.Multiple
        (
            () => Assert.Contains(nameof(Event.Name), responseBody.Errors),
            () => Assert.Contains(nameof(Event.Description), responseBody.Errors),
            () => Assert.Contains(nameof(Event.Location), responseBody.Errors),
            () => Assert.Contains(nameof(Event.StartTime), responseBody.Errors),
            () => Assert.Contains(nameof(Event.EndTime), responseBody.Errors)
        );
    }

    [Fact]
    public async Task PostEvent_ReturnsResponseWithCreatedStatusCode_WhenResourceIsValid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, ValidEvent, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}