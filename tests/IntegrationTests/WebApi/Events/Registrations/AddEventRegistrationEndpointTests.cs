using System.Net;
using System.Net.Http.Json;

using Events.Domain;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.Domain.Events.EventEntityBuilder;
using static Events.WebApi.Events.Registrations.RegistrationResourceBuilder;

using EventEntity = Events.Domain.Events.Event;
using IEventsRepository = Events.Domain.Events.IRepository;
using IRegistrationsRepository = Events.Domain.Events.Registrations.IRepository;
using RegistrationEmailAddress = Events.Domain.Events.Registrations.EmailAddress;
using RegistrationEntity = Events.Domain.Events.Registrations.Registration;
using RegistrationName = Events.Domain.Events.Registrations.Name;
using RegistrationPhoneNumber = Events.Domain.Events.Registrations.PhoneNumber;
using RegistrationResource = Events.WebApi.Events.Registrations.Registration;

namespace Events.WebApi.Events.Registrations;

public sealed class AddEventRegistrationEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly string RequestUrl = Endpoints.AddEventRegistrationRoute.Replace("{id}", AnEventIdValue);

    private static readonly RegistrationResource InvalidRegistrationResource = new();
    private static readonly RegistrationResource ValidRegistrationResource = ARegistrationResource.Build();

    private readonly IEventsRepository _eventsRepositoryMock;
    private readonly IRegistrationsRepository _registrationsRepositoryMock;
    private readonly HttpClient _httpClient;

    public AddEventRegistrationEndpointTests(WebApplicationFactory<Program> factory)
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
                })
        )
        .CreateClient();
    }

    [Fact]
    public async Task PostRegistration_ReturnsResponseWithBadRequestStatusCode_WhenResourceIsInvalid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, InvalidRegistrationResource, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostRegistration_ReturnsValidationErrorsInResponseBody_WhenResourceIsInvalid()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, InvalidRegistrationResource, TestContext.Current.CancellationToken);

        ValidationProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody?.Errors);
        Assert.Multiple
        (
            () => Assert.Contains(nameof(RegistrationResource.Name), responseBody.Errors),
            () => Assert.Contains(nameof(RegistrationResource.PhoneNumber), responseBody.Errors),
            () => Assert.Contains(nameof(RegistrationResource.EmailAddress), responseBody.Errors)
        );
    }

    [Fact]
    public async Task PostRegistration_ReturnsResponseWithNotFoundStatusCode_WhenEventWithIdFromUrlIsNotFound()
    {
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostRegistration_ReturnsErrorInResponseBody_WhenEventWithIdFromUrlIsNotFound()
    {
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        ProblemDetails? responseBody = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.NotNull(responseBody);
        Assert.Multiple
        (
            () => Assert.NotEmpty(responseBody.Title!),
            () => Assert.Equal(StatusCodes.Status404NotFound, responseBody.Status)
        );
    }

    [Fact]
    public async Task PostRegistration_SavesRegistration_WhenResourceIsValid()
    {
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns
            (
                ANewEventEntity
                    .WithId(AnEventIdValue)
                    .Build()
            );

        RegistrationEntity? savedRegistration = null;
        await _registrationsRepositoryMock.Save
        (
            Arg.Do<RegistrationEntity>(@registration => savedRegistration = registration),
            Arg.Any<CancellationToken>()
        );

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        Assert.NotNull(savedRegistration);
        Assert.Multiple
        (
            () => Assert.NotNull(savedRegistration.Id),
            () => Assert.Equal(new Id(AnEventIdValue), savedRegistration.EventId),
            () => Assert.Equal(new RegistrationName(ValidRegistrationResource.Name), savedRegistration.Name),
            () => Assert.Equal(new RegistrationPhoneNumber(ValidRegistrationResource.PhoneNumber), savedRegistration.PhoneNumber),
            () => Assert.Equal(new RegistrationEmailAddress(ValidRegistrationResource.EmailAddress), savedRegistration.EmailAddress)
        );
    }

    [Fact]
    public async Task PostRegistration_ReturnsResponseWithCreatedStatusCode_WhenResourceIsValid()
    {
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns
            (
                ANewEventEntity
                    .WithId(AnEventIdValue)
                    .Build()
            );

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsResponseWithRegistrationBody_WhenResourceIsValid()
    {
        _eventsRepositoryMock.Get(new Id(AnEventIdValue), Arg.Any<CancellationToken>())
            .Returns
            (
                ANewEventEntity
                    .WithId(AnEventIdValue)
                    .Build()
            );

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        RegistrationResource? registration = await response.Content.ReadFromJsonAsync<RegistrationResource>(TestContext.Current.CancellationToken);

        Assert.NotNull(registration);
        Assert.Multiple
        (
            () => Assert.NotEmpty(registration.Id!),
            () => Assert.Equal(AnEventIdValue, registration.EventId),
            () => Assert.Equal(ValidRegistrationResource.Name, registration.Name),
            () => Assert.Equal(ValidRegistrationResource.PhoneNumber, registration.PhoneNumber),
            () => Assert.Equal(ValidRegistrationResource.EmailAddress, registration.EmailAddress)
        );
    }
}