using System.Net;
using System.Net.Http.Json;

using Events.Domain.Events;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.WebApi.Common.Events.EventResourceBuilder;

using EventEntity = Events.Domain.Events.Event;
using EventResource = Events.WebApi.Events.Event;

namespace Events.WebApi.Events;

public sealed class CreateEventEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly EventResource ValidEvent = AnEventResource.Build();

    private readonly IEventsRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public CreateEventEndpointTests(WebApplicationFactory<Program> factory)
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
        EventResource invalidEvent = AnEventResource
            .WithoutName()
            .WithoutDescription()
            .WithoutLocation()
            .WithStartTime(now)
            .WithEndTime(now)
            .Build();

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

        Assert.Multiple
        (
            () => Assert.NotNull(savedEvent.Id),
            () => Assert.Equal(ValidEvent.Name, savedEvent.Name.ToString()),
            () => Assert.Equal(ValidEvent.Description, savedEvent.Description.ToString()),
            () => Assert.Equal(ValidEvent.Location, savedEvent.Location.ToString()),
            () => Assert.Equal(ValidEvent.StartTime, savedEvent.StartTime.Value),
            () => Assert.Equal(ValidEvent.EndTime, savedEvent.EndTime.Value)
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
            () => Assert.Equal(ValidEvent.Name.ToString(), @event.Name),
            () => Assert.Equal(ValidEvent.Description.ToString(), @event.Description),
            () => Assert.Equal(ValidEvent.Location.ToString(), @event.Location),
            () => Assert.Equal(ValidEvent.StartTime, @event.StartTime),
            () => Assert.Equal(ValidEvent.EndTime, @event.EndTime)
        );
    }
}