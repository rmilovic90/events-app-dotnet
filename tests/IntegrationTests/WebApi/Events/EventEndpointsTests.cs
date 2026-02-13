using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Events.WebApi.Events;

public sealed class EventEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static Event ValidEvent => new()
    {
        Name = "Test",
        Description = "The test event.",
        Location = "Novi Sad, Serbia",
        StartTime = DateTime.UtcNow.Date.AddDays(2),
        EndTime = DateTime.UtcNow
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
        Event invalidEvent = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, invalidEvent, TestContext.Current.CancellationToken);

        ValidationProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody?.Errors);
        Assert.Multiple
        (
            () => Assert.Contains(nameof(Event.Name), responseBody.Errors),
            () => Assert.Contains(nameof(Event.Description), responseBody.Errors),
            () => Assert.Contains(nameof(Event.Location), responseBody.Errors),
            () => Assert.Contains(nameof(Event.StartTime), responseBody.Errors)
        );
    }

    [Fact]
    public async Task Post_ReturnsResponseWithCreatedStatusCode_WhenResourceIsValid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, ValidEvent, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}