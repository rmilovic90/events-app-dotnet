using System.Net;
using System.Net.Http.Json;

using Events.Domain.Events;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.WebApi.Common.Events.EventEntityBuilder;

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
        EventEntity firstEvent = ANewEventEntity
            .WithName("Test 1")
            .WithDescription("Test event 1.")
            .WithLocation("Novi Sad, Serbia")
            .Build();
        EventEntity secondEvent = ANewEventEntity
            .WithName("Test 2")
            .WithDescription("Test event 2.")
            .WithLocation("Novi Sad, Serbia")
            .Build();

        _repositoryMock.GetAll(Arg.Any<CancellationToken>())
            .Returns([firstEvent, secondEvent]);

        HttpResponseMessage response = await _httpClient.GetAsync(Endpoints.GetAllRoute, TestContext.Current.CancellationToken);

        IEnumerable<EventResource>? events = await response.Content.ReadFromJsonAsync<IEnumerable<EventResource>>(TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new[]
            {
                EventResource.FromEntity(firstEvent),
                EventResource.FromEntity(secondEvent)
            },
            events
        );
    }
}