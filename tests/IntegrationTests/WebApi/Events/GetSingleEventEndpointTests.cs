using System.Net;
using System.Net.Http.Json;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using EventEntity = Events.Domain.Events.Event;
using EventResource = Events.WebApi.Events.Event;

namespace Events.WebApi.Events;

public sealed class GetSingleEventEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string EventId = "019c770f-52d0-7656-9298-adeecf45987a";

    private static readonly string RequestUrl = Endpoints.GetSingleRoute.Replace("{id}", EventId);

    private static readonly DateTime UtcTomorrow = DateTime.UtcNow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    private static readonly DateTimeOffset StartTimeValue = new(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 14, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow));
    private static readonly DateTimeOffset EndTimeValue = new(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 15, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow));

    private readonly IEventsRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public GetSingleEventEndpointTests(WebApplicationFactory<Program> factory)
    {
        _repositoryMock = Substitute.For<IEventsRepository>();

        _httpClient = factory.WithWebHostBuilder
        (
            builder => builder.ConfigureTestServices(services => services.AddTransient(_ => _repositoryMock))
        )
        .CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsResponseWithNotFoundStatusCode_WhenEventWithIdFromUrlIsNotFound()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsResponseWithOkStatusCode_WhenEventWithIdFromUrlIsFound()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns
            (
                EventEntity.Of
                (
                    new Id(),
                    new Name("Test 1"),
                    new Description("Test event 1."),
                    new Location("Novi Sad, Serbia"),
                    StartTime.Of(StartTimeValue),
                    EndTime.Of(EndTimeValue, StartTime.Of(StartTimeValue))
                )
            );

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsResponseWithEventBody_WhenEventWithIdFromUrlIsFound()
    {
        EventEntity @event = EventEntity.Of
        (
            new Id(),
            new Name("Test 1"),
            new Description("Test event 1."),
            new Location("Novi Sad, Serbia"),
            StartTime.Of(StartTimeValue),
            EndTime.Of(EndTimeValue, StartTime.Of(StartTimeValue))
        );

        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(@event);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        EventResource? eventResource = await response.Content.ReadFromJsonAsync<EventResource>(TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new EventResource
            {
                Id = @event.Id.ToString(),
                Name = @event.Name.ToString(),
                Description = @event.Description.ToString(),
                Location = @event.Location.ToString(),
                StartTime = @event.StartTime.Value,
                EndTime = @event.EndTime.Value
            },
            eventResource
        );
    }
}