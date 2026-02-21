using System.Net;
using System.Net.Http.Json;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using EventEntity = Events.Domain.Events.Event;
using EventRegistrationEntity = Events.Domain.Events.Registration;
using EventRegistrationResource = Events.WebApi.Events.Registration;

namespace Events.WebApi.Events;

public sealed class GetAllEventRegistrationsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string EventId = "019c770f-52d0-7656-9298-adeecf45987a";

    private static readonly string RequestUrl = Endpoints.AddEventRegistrationRoute.Replace("{id}", EventId);

    private static readonly DateTime UtcTomorrow = DateTime.UtcNow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    private static readonly DateTimeOffset StartTimeValue = new(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 14, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow));
    private static readonly DateTimeOffset EndTimeValue = new(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 15, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow));

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
    public async Task Get_ReturnsResponseWithEventRegistrationsListBody_WhenEventWithIdFromUrlIsFound()
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

        EventRegistrationEntity firstEventRegistration = EventRegistrationEntity.New
        (
            new Id(EventId),
            new RegistrationName("Jane Doe"),
            new RegistrationPhoneNumber("+38155555555"),
            new RegistrationEmailAddress("jane.doe@email.com")
        );
        EventRegistrationEntity secondEventRegistration = EventRegistrationEntity.New
        (
            new Id(EventId),
            new RegistrationName("John Doe"),
            new RegistrationPhoneNumber("+38155666666"),
            new RegistrationEmailAddress("john.doe@email.com")
        );

        _repositoryMock.GetAllRegistrations(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns([firstEventRegistration, secondEventRegistration]);

        HttpResponseMessage response = await _httpClient.GetAsync(RequestUrl, TestContext.Current.CancellationToken);

        IEnumerable<EventRegistrationResource>? eventRegistrations = await response.Content.ReadFromJsonAsync<IEnumerable<EventRegistrationResource>>(TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new[]
            {
                new EventRegistrationResource
                {
                    Id = firstEventRegistration.Id.ToString(),
                    EventId = firstEventRegistration.EventId.ToString(),
                    Name = firstEventRegistration.Name.ToString(),
                    PhoneNumber = firstEventRegistration.PhoneNumber.ToString(),
                    EmailAddress = firstEventRegistration.EmailAddress.ToString()
                },
                new EventRegistrationResource
                {
                    Id = secondEventRegistration.Id.ToString(),
                    EventId = secondEventRegistration.EventId.ToString(),
                    Name = secondEventRegistration.Name.ToString(),
                    PhoneNumber = secondEventRegistration.PhoneNumber.ToString(),
                    EmailAddress = secondEventRegistration.EmailAddress.ToString()
                }
            },
            eventRegistrations
        );
    }
}