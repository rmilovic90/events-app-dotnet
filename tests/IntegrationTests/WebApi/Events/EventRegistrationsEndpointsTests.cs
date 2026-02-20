using System.Net;
using System.Net.Http.Json;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

using NSubstitute;

using EventEntity = Events.Domain.Events.Event;
using EventRegistrationResource = Events.WebApi.Events.Registration;
using RegistrationEntity = Events.Domain.Events.Registration;

namespace Events.WebApi.Events;

public class EventRegistrationsEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string EventId = "019c770f-52d0-7656-9298-adeecf45987a";
    private const string Name = "Jane Doe";
    private const string PhoneNumber = "+38155555555";
    private const string EmailAddress = "jane.doe@email.com";

    private static readonly string RequestUrl = Endpoints.AddRegistrationRoute.Replace("{id}", EventId);

    private static readonly EventRegistrationResource ValidEventRegistration = new()
    {
        Name = Name,
        PhoneNumber = PhoneNumber,
        EmailAddress = EmailAddress
    };

    private readonly IEventsRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public EventRegistrationsEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _repositoryMock = Substitute.For<IEventsRepository>();

        _httpClient = factory.WithWebHostBuilder
        (
            builder => builder.ConfigureTestServices(services => services.AddTransient(_ => _repositoryMock))
        )
        .CreateClient();
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsResponseWithBadRequestStatusCode_WhenResourceIsInvalid()
    {
        EventRegistrationResource invalidEventRegistration = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, invalidEventRegistration, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsValidationErrorsInResponseBody_WhenResourceIsInvalid()
    {
        EventRegistrationResource invalidEventRegistration = new();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, invalidEventRegistration, TestContext.Current.CancellationToken);

        ValidationProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody?.Errors);
        Assert.Multiple
        (
            () => Assert.Contains(nameof(EventRegistrationResource.Name), responseBody.Errors),
            () => Assert.Contains(nameof(EventRegistrationResource.PhoneNumber), responseBody.Errors),
            () => Assert.Contains(nameof(EventRegistrationResource.EmailAddress), responseBody.Errors)
        );
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsResponseWithNotFoundStatusCode_WhenEventWithIdFromUrlIsNotFound()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidEventRegistration, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsErrorInResponseBody_WhenEventWithIdFromUrlIsNotFound()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidEventRegistration, TestContext.Current.CancellationToken);

        ProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody);
        Assert.Multiple
        (
            () => Assert.NotEmpty(responseBody.Title!),
            () => Assert.Equal(StatusCodes.Status404NotFound, responseBody.Status)
        );
    }

    [Fact]
    public async Task PostEventRegistration_SavesRegistration_WhenResourceIsValid()
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        DateTimeOffset utcTomorrow = utcNow.AddDays(1);
        DateTimeOffset utcDayAfterTomorrow = utcTomorrow.AddDays(1);
        StartTime startTime = StartTime.New(utcTomorrow, new FakeTimeProvider(utcNow));
        EventEntity @event = EventEntity.Of
        (
            new Id(),
            new Name("Test"),
            new Description("Test event."),
            new Location("Novi Sad, Serbia"),
            startTime,
            EndTime.Of(utcDayAfterTomorrow, startTime)
        );

        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(@event);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidEventRegistration, TestContext.Current.CancellationToken);

        await _repositoryMock.Received(1)
            .Save
            (
                Arg.Any<EventEntity>(),
                Arg.Any<CancellationToken>()
            );

        RegistrationEntity savedRegistration = Assert.Single(@event.PendingRegistrations);
        Assert.Multiple
        (
            () => Assert.NotNull(savedRegistration.Id),
            () => Assert.Equal(new Id(EventId), savedRegistration.EventId),
            () => Assert.Equal(new RegistrationName(Name), savedRegistration.Name),
            () => Assert.Equal(new RegistrationPhoneNumber(PhoneNumber), savedRegistration.PhoneNumber),
            () => Assert.Equal(new RegistrationEmailAddress(EmailAddress), savedRegistration.EmailAddress)
        );
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsResponseWithCreatedStatusCode_WhenResourceIsValid()
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        DateTimeOffset utcTomorrow = utcNow.AddDays(1);
        DateTimeOffset utcDayAfterTomorrow = utcTomorrow.AddDays(1);
        StartTime startTime = StartTime.New(utcTomorrow, new FakeTimeProvider(utcNow));
        EventEntity @event = EventEntity.Of
        (
            new Id(),
            new Name("Test"),
            new Description("Test event."),
            new Location("Novi Sad, Serbia"),
            startTime,
            EndTime.Of(utcDayAfterTomorrow, startTime)
        );

        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(@event);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidEventRegistration, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsResponseWithRegistrationBody_WhenResourceIsValid()
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        DateTimeOffset utcTomorrow = utcNow.AddDays(1);
        DateTimeOffset utcDayAfterTomorrow = utcTomorrow.AddDays(1);
        StartTime startTime = StartTime.New(utcTomorrow, new FakeTimeProvider(utcNow));
        EventEntity @event = EventEntity.Of
        (
            new Id(),
            new Name("Test"),
            new Description("Test event."),
            new Location("Novi Sad, Serbia"),
            startTime,
            EndTime.Of(utcDayAfterTomorrow, startTime)
        );

        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(@event);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidEventRegistration, TestContext.Current.CancellationToken);

        EventRegistrationResource? registration = await response.Content.ReadFromJsonAsync<EventRegistrationResource>(TestContext.Current.CancellationToken);

        Assert.NotNull(@registration);
        Assert.Multiple
        (
            () => Assert.NotEmpty(registration.Id!),
            () => Assert.Equal(EventId, registration.EventId),
            () => Assert.Equal(Name, registration.Name),
            () => Assert.Equal(PhoneNumber, registration.PhoneNumber),
            () => Assert.Equal(EmailAddress, registration.EmailAddress)
        );
    }
}