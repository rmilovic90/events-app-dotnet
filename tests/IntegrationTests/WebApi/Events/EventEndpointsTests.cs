using System.Net;
using System.Net.Http.Json;

using Events.Domain.Events;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using EventEntity = Events.Domain.Events.Event;
using EventResource = Events.WebApi.Events.Event;

namespace Events.WebApi.Events;

public sealed class EventEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Name = "Test";
    private const string Description = "Test event.";
    private const string Location = "Novi Sad, Serbia";

    private static readonly DateTime UtcTomorrow = DateTime.UtcNow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    private static readonly DateTimeOffset StartTime = new(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 14, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow));
    private static readonly DateTimeOffset EndTime = new(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 15, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow));

    private static readonly EventResource ValidEvent = new()
    {
        Name = Name,
        Description = Description,
        Location = Location,
        StartTime = StartTime,
        EndTime = EndTime
    };

    private readonly IEventsRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public EventEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _repositoryMock = Substitute.For<IEventsRepository>();

        _httpClient = factory.WithWebHostBuilder
        (
            builder => builder.ConfigureTestServices
            (
                services =>
                {
                    services.AddTransient(_ => _repositoryMock);
                    services.AddAuthentication(TestAuthHandler.TestAuthenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>
                        (
                            TestAuthHandler.TestAuthenticationScheme,
                            options => { }
                        );
                }
            )
        )
        .CreateClient();
    }

    [Fact]
    public async Task PostEvent_ReturnsResponseWithBadRequestStatusCode_WhenResourceIsInvalid()
    {
        EventResource invalidEvent = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, invalidEvent, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostEvent_ReturnsValidationErrorsInResponseBody_WhenResourceIsInvalid()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        EventResource invalidEvent = new()
        {
            StartTime = now,
            EndTime = now
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, invalidEvent, TestContext.Current.CancellationToken);

        ValidationProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody?.Errors);
        Assert.Multiple
        (
            () => Assert.Contains(nameof(EventResource.Name), responseBody.Errors),
            () => Assert.Contains(nameof(EventResource.Description), responseBody.Errors),
            () => Assert.Contains(nameof(EventResource.Location), responseBody.Errors),
            () => Assert.Contains(nameof(EventResource.StartTime), responseBody.Errors),
            () => Assert.Contains(nameof(EventResource.EndTime), responseBody.Errors)
        );
    }

    [Fact]
    public async Task PostEvent_SavesEvent_WhenResourceIsValid()
    {
        EventEntity savedEvent = null!;
        await _repositoryMock.Save
        (
            Arg.Do<EventEntity>(@event => savedEvent = @event),
            Arg.Any<CancellationToken>()
        );

        await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, ValidEvent, TestContext.Current.CancellationToken);

        await _repositoryMock.Received(1)
            .Save
            (
                Arg.Any<EventEntity>(),
                Arg.Any<CancellationToken>()
            );

        Assert.Multiple
        (
            () => Assert.NotNull(savedEvent.Id),
            () => Assert.Equal(Name, savedEvent.Name.ToString()),
            () => Assert.Equal(Description, savedEvent.Description.ToString()),
            () => Assert.Equal(Location, savedEvent.Location.ToString()),
            () => Assert.Equal(StartTime, savedEvent.StartTime.Value),
            () => Assert.Equal(EndTime, savedEvent.EndTime.Value)
        );
    }

    [Fact]
    public async Task PostEvent_ReturnsResponseWithCreatedStatusCode_WhenResourceIsValid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, ValidEvent, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostEvent_ReturnsResponseWithEventBody_WhenResourceIsValid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(Endpoints.CreateRoute, ValidEvent, TestContext.Current.CancellationToken);

        EventResource? @event = await response.Content.ReadFromJsonAsync<EventResource>(TestContext.Current.CancellationToken);

        Assert.NotNull(@event);
        Assert.Multiple
        (
            () => Assert.NotEmpty(@event.Id!),
            () => Assert.Equal(Name, @event.Name),
            () => Assert.Equal(Description, @event.Description),
            () => Assert.Equal(Location, @event.Location),
            () => Assert.Equal(StartTime, @event.StartTime),
            () => Assert.Equal(EndTime, @event.EndTime)
        );
    }
}