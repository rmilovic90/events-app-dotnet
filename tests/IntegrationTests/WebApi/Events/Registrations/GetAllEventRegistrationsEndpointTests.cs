using System.Net;
using System.Net.Http.Json;

using Events.Domain;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.Domain.Events.EventEntityBuilder;
using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;

using EventEntity = Events.Domain.Events.Event;
using IEventsRepository = Events.Domain.Events.IRepository;
using IRegistrationsRepository = Events.Domain.Events.Registrations.IRepository;
using RegistrationEntity = Events.Domain.Events.Registrations.Registration;
using RegistrationResource = Events.WebApi.Events.Registrations.Registration;

namespace Events.WebApi.Events.Registrations;

public sealed class GetAllEventRegistrationsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly string RequestUrl = Endpoints.AddEventRegistrationRoute.Replace("{id}", AnEventIdValue);

    private readonly IEventsRepository _eventsRepositoryMock;
    private readonly IRegistrationsRepository _registrationsRepositoryMock;
    private readonly HttpClient _httpClient;

    public GetAllEventRegistrationsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _eventsRepositoryMock = Substitute.For<IEventsRepository>();
        _registrationsRepositoryMock = Substitute.For<IRegistrationsRepository>();

        _httpClient = factory.WithWebHostBuilder
        (
            builder => builder.ConfigureTestServices
            (
                services =>
                {
                    services.AddTransient(_ => _eventsRepositoryMock);
                    services.AddTransient(_ => _registrationsRepositoryMock);
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
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsResponseWithOkStatusCode_WhenEventWithIdFromUrlIsFound()
    {
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
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
    public async Task Get_ReturnsResponseWithEventRegistrationsListBody_WhenEventWithIdFromUrlIsFound()
    {
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns
            (
                ANewEventEntity
                    .WithId(AnEventIdValue)
                    .Build()
            );

        RegistrationEntity firstEventRegistration = ANewRegistrationEntity
            .WithEventId(AnEventIdValue)
            .WithName("Jane Doe")
            .WithPhoneNumber("+38155555555")
            .WithEmailAddress("jane.doe@email.com")
            .Build();
        RegistrationEntity secondEventRegistration = ANewRegistrationEntity
            .WithEventId(AnEventIdValue)
            .WithName("John Doe")
            .WithPhoneNumber("+38155666666")
            .WithEmailAddress("john.doe@email.com")
            .Build();

        _registrationsRepositoryMock.GetAll(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns([firstEventRegistration, secondEventRegistration]);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        IEnumerable<RegistrationResource>? eventRegistrations = await response.Content.ReadFromJsonAsync<IEnumerable<RegistrationResource>>(TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new[]
            {
                RegistrationResource.FromEntity(firstEventRegistration),
                RegistrationResource.FromEntity(secondEventRegistration)
            },
            eventRegistrations
        );
    }
}