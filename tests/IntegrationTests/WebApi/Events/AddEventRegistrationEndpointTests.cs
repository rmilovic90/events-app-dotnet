using System.Net;
using System.Net.Http.Json;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using static Events.WebApi.Common.Events.EventEntityBuilder;
using static Events.WebApi.Common.Events.Registrations.RegistrationResourceBuilder;

using EventEntity = Events.Domain.Events.Event;
using RegistrationEntity = Events.Domain.Events.Registration;
using RegistrationResource = Events.WebApi.Events.Registration;

namespace Events.WebApi.Events;

public class AddEventRegistrationEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string EventId = "019c770f-52d0-7656-9298-adeecf45987a";

    private static readonly string RequestUrl = Endpoints.AddEventRegistrationRoute.Replace("{id}", EventId);

    private static readonly RegistrationResource InvalidRegistrationResource = new();
    private static readonly RegistrationResource ValidRegistrationResource = ARegistrationResource.Build();

    private readonly IEventsRepository _repositoryMock;
    private readonly HttpClient _httpClient;

    public AddEventRegistrationEndpointTests(WebApplicationFactory<Program> factory)
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
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, InvalidRegistrationResource, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsValidationErrorsInResponseBody_WhenResourceIsInvalid()
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
    public async Task PostEventRegistration_ReturnsResponseWithNotFoundStatusCode_WhenEventWithIdFromUrlIsNotFound()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsErrorInResponseBody_WhenEventWithIdFromUrlIsNotFound()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
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
    public async Task PostEventRegistration_SavesRegistration_WhenResourceIsValid()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(ANewEventEntity.Build());

        EventEntity? savedEvent = null;
        await _repositoryMock.Save
        (
            Arg.Do<EventEntity>(@event => savedEvent = @event),
            Arg.Any<CancellationToken>()
        );

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        RegistrationEntity savedRegistration = Assert.Single(savedEvent?.PendingRegistrations ?? []);
        Assert.Multiple
        (
            () => Assert.NotNull(savedRegistration.Id),
            () => Assert.Equal(new Id(EventId), savedRegistration.EventId),
            () => Assert.Equal(new RegistrationName(ValidRegistrationResource.Name), savedRegistration.Name),
            () => Assert.Equal(new RegistrationPhoneNumber(ValidRegistrationResource.PhoneNumber), savedRegistration.PhoneNumber),
            () => Assert.Equal(new RegistrationEmailAddress(ValidRegistrationResource.EmailAddress), savedRegistration.EmailAddress)
        );
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsResponseWithCreatedStatusCode_WhenResourceIsValid()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(ANewEventEntity.Build());

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostEventRegistration_ReturnsResponseWithRegistrationBody_WhenResourceIsValid()
    {
        _repositoryMock.Get(new Id(EventId), Arg.Any<CancellationToken>())
            .Returns(ANewEventEntity.Build());

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(RequestUrl, ValidRegistrationResource, TestContext.Current.CancellationToken);

        RegistrationResource? registration = await response.Content.ReadFromJsonAsync<RegistrationResource>(TestContext.Current.CancellationToken);

        Assert.NotNull(registration);
        Assert.Multiple
        (
            () => Assert.NotEmpty(registration.Id!),
            () => Assert.Equal(EventId, registration.EventId),
            () => Assert.Equal(ValidRegistrationResource.Name, registration.Name),
            () => Assert.Equal(ValidRegistrationResource.PhoneNumber, registration.PhoneNumber),
            () => Assert.Equal(ValidRegistrationResource.EmailAddress, registration.EmailAddress)
        );
    }
}