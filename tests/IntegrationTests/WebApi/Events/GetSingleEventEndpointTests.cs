using System.Net;
using System.Net.Http.Json;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.Domain.Events.EventEntityBuilder;

using EventEntity = Events.Domain.Events.Event;
using EventResource = Events.WebApi.Events.Event;

namespace Events.WebApi.Events;

public sealed class GetSingleEventEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly string RequestUrl = Endpoints.GetSingleRoute.Replace("{id}", AnEventIdValue);

    private readonly IRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public GetSingleEventEndpointTests(WebApplicationFactory<Program> factory)
    {
        _repositoryMock = Substitute.For<IRepository>();

        _httpClient = factory.WithWebHostBuilder
        (
            builder => builder.ConfigureTestServices(services => services.AddTransient(_ => _repositoryMock))
        )
        .CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsResponseWithNotFoundStatusCode_WhenEventWithIdFromUrlIsNotFound()
    {
        _repositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsResponseWithOkStatusCode_WhenEventWithIdFromUrlIsFound()
    {
        _repositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns
            (
                ANewEventEntity
                    .WithId(AnEventIdValue)
                    .Build()
            );

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsResponseWithEventBody_WhenEventWithIdFromUrlIsFound()
    {
        EventEntity @event = ANewEventEntity
            .WithId(AnEventIdValue)
            .Build();

        _repositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns(@event);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        EventResource? eventResource = await response.Content.ReadFromJsonAsync<EventResource>(TestContext.Current.CancellationToken);

        Assert.Equivalent(EventResource.FromEntity(@event), eventResource);
    }
}