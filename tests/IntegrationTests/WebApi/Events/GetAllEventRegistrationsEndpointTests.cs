using System.Net;
using System.Net.Http.Json;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.Domain.Events.EventEntityBuilder;
using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;

using EventEntity = Events.Domain.Events.Event;
using RegistrationEntity = Events.Domain.Events.Registration;
using RegistrationResource = Events.WebApi.Events.Registration;

namespace Events.WebApi.Events;

public sealed class GetAllEventRegistrationsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly string RequestUrl = Endpoints.AddEventRegistrationRoute.Replace("{id}", AnEventIdValue);

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
    public async Task Get_ReturnsResponseWithEventRegistrationsListBody_WhenEventWithIdFromUrlIsFound()
    {
        _repositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
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

        _repositoryMock.GetAllRegistrations(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
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