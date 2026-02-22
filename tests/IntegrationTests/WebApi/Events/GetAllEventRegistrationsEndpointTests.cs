using System.Net;
using System.Net.Http.Json;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.WebApi.Common.Events.EventEntityBuilder;
using static Events.WebApi.Common.Events.Registrations.RegistrationEntityBuilder;

using EventEntity = Events.Domain.Events.Event;
using EventRegistrationEntity = Events.Domain.Events.Registration;
using EventRegistrationResource = Events.WebApi.Events.Registration;

namespace Events.WebApi.Events;

public sealed class GetAllEventRegistrationsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string EventId = "019c770f-52d0-7656-9298-adeecf45987a";

    private static readonly string RequestUrl = Endpoints.AddEventRegistrationRoute.Replace("{id}", EventId);

    private readonly IEventsRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public GetAllEventRegistrationsEndpointTests(WebApplicationFactory<Program> factory)
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
            .Returns(ANewEventEntity.Build());

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsResponseWithEventRegistrationsListBody_WhenEventWithIdFromUrlIsFound()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(ANewEventEntity.Build());

        EventRegistrationEntity firstEventRegistration = ANewRegistrationEntity
            .WithEventId(EventId)
            .WithName("Jane Doe")
            .WithPhoneNumber("+38155555555")
            .WithEmailAddress("jane.doe@email.com")
            .Build();
        EventRegistrationEntity secondEventRegistration = ANewRegistrationEntity
            .WithEventId(EventId)
            .WithName("John Doe")
            .WithPhoneNumber("+38155666666")
            .WithEmailAddress("john.doe@email.com")
            .Build();

        _repositoryMock.GetAllRegistrations(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns([firstEventRegistration, secondEventRegistration]);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        IEnumerable<EventRegistrationResource>? eventRegistrations = await response.Content.ReadFromJsonAsync<IEnumerable<EventRegistrationResource>>(TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new[]
            {
                EventRegistrationResource.FromEntity(firstEventRegistration),
                EventRegistrationResource.FromEntity(secondEventRegistration)
            },
            eventRegistrations
        );
    }
}