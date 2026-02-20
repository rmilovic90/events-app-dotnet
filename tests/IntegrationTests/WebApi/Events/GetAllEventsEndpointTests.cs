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

public sealed class GetAllEventsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IEventsRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public GetAllEventsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _repositoryMock = Substitute.For<IEventsRepository>();

        _httpClient = factory.WithWebHostBuilder
        (
            builder => builder.ConfigureTestServices(services => services.AddTransient(_ => _repositoryMock))
        )
        .CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsResponseWithOkStatusCode()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(Endpoints.GetAllRoute, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsResponseWithEventsListBody()
    {
        DateTime utcTomorrow = DateTime.UtcNow.AddDays(1);
        TimeZoneInfo centralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTimeOffset startTime = new(utcTomorrow.Year, utcTomorrow.Month, utcTomorrow.Day, 14, 0, 0, centralEuropeanTimeZone.GetUtcOffset(utcTomorrow));
        DateTimeOffset endTime = new(utcTomorrow.Year, utcTomorrow.Month, utcTomorrow.Day, 15, 0, 0, centralEuropeanTimeZone.GetUtcOffset(utcTomorrow));
        EventEntity firstEvent = EventEntity.Of
        (
            new Id(),
            new Name("Test 1"),
            new Description("Test event 1."),
            new Location("Novi Sad, Serbia"),
            StartTime.Of(startTime),
            EndTime.Of(endTime, StartTime.Of(startTime))
        );
        EventEntity secondEvent = EventEntity.Of
        (
            new Id(),
            new Name("Test 2"),
            new Description("Test event 2."),
            new Location("Novi Sad, Serbia"),
            StartTime.Of(startTime),
            EndTime.Of(endTime, StartTime.Of(startTime))
        );

        _repositoryMock.GetAll(Arg.Any<CancellationToken>())
            .Returns([firstEvent, secondEvent]);

        HttpResponseMessage response = await _httpClient.GetAsync(Endpoints.GetAllRoute, TestContext.Current.CancellationToken);

        IEnumerable<EventResource>? events = await response.Content.ReadFromJsonAsync<IEnumerable<EventResource>>(TestContext.Current.CancellationToken);

        Assert.NotNull(events);
        Assert.Equivalent
        (
            new[]
            {
                new EventResource
                {
                    Id = firstEvent.Id.ToString(),
                    Name = firstEvent.Name.ToString(),
                    Description = firstEvent.Description.ToString(),
                    Location = firstEvent.Location.ToString(),
                    StartTime = firstEvent.StartTime.Value,
                    EndTime = firstEvent.EndTime.Value
                },
                new EventResource
                {
                    Id = secondEvent.Id.ToString(),
                    Name = secondEvent.Name.ToString(),
                    Description = secondEvent.Description.ToString(),
                    Location = secondEvent.Location.ToString(),
                    StartTime = secondEvent.StartTime.Value,
                    EndTime = secondEvent.EndTime.Value
                }
            },
            events
        );
    }
}