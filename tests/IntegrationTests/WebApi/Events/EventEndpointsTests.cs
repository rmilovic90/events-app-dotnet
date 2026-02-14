using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Events.WebApi.Events;

public sealed class EventEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly DateTime _utcTomorrow = DateTime.UtcNow.Date.AddDays(1);
    private static readonly TimeZoneInfo _centralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private static Event ValidEvent => new()
    {
        Name = "Test",
        Description = "The test event.",
        Location = "Novi Sad, Serbia",
        StartTime = new DateTimeOffset(_utcTomorrow.Year, _utcTomorrow.Month, _utcTomorrow.Day, 14, 0, 0, _centralEuropeanTimeZone.GetUtcOffset(_utcTomorrow)),
        EndTime = new DateTimeOffset(_utcTomorrow.Year, _utcTomorrow.Month, _utcTomorrow.Day, 15, 0, 0, _centralEuropeanTimeZone.GetUtcOffset(_utcTomorrow))
    };

    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task Post_ReturnsResponseWithBadRequestStatusCode_WhenResourceIsInvalid()
    {
        Event invalidEvent = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, invalidEvent, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsValidationErrorsInResponseBody_WhenResourceIsInvalid()
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
    public async Task Post_ReturnsResponseWithCreatedStatusCode_WhenResourceIsValid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, ValidEvent, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}