using System.Net.Mime;

using Events.Domain;

using Microsoft.AspNetCore.Mvc;

using EventEntity = Events.Domain.Events.Event;
using EventsEndpoints = Events.WebApi.Events.Endpoints;
using IEventsRepository = Events.Domain.Events.IRepository;
using IRegistrationsRepository = Events.Domain.Events.Registrations.IRepository;
using RegistrationEntity = Events.Domain.Events.Registrations.Registration;
using RegistrationResource = Events.WebApi.Events.Registrations.Registration;

namespace Events.WebApi.Events.Registrations;

internal static class Endpoints
{
    internal const string GetAllEventRegistrationsRoute = $"{EventsEndpoints.GetSingleRoute}/registrations";
    internal const string AddEventRegistrationRoute = $"{EventsEndpoints.GetSingleRoute}/registrations";

    private static async Task<IResult> GetAllEventRegistrations
    (
        [FromRoute(Name = "id")] string eventId,
        IEventsRepository eventsRepository,
        IRegistrationsRepository registrationsRepository,
        CancellationToken cancellationToken
    )
    {
        EventEntity? @event = await eventsRepository.Get(new Id(eventId), cancellationToken);

        if (@event is null)
            return TypedResults.Problem
            (
                title: $"Event with ID '{eventId}' does not exist.",
                statusCode: StatusCodes.Status404NotFound
            );

        IReadOnlyList<RegistrationEntity> registrations = await registrationsRepository.GetAll(new Id(eventId), cancellationToken);

        return TypedResults.Ok(registrations.Select(RegistrationResource.FromEntity));
    }

    private static async Task<IResult> AddEventRegistration
    (
        [FromRoute(Name = "id")] string eventId,
        [FromBody] RegistrationResource registration,
        IEventsRepository eventsRepository,
        IRegistrationsRepository registrationsRepository,
        CancellationToken cancellationToken
    )
    {
        EventEntity? @event = await eventsRepository.Get(new Id(eventId), cancellationToken);

        if (@event is null)
            return TypedResults.Problem
            (
                title: $"Event with ID '{eventId}' does not exist.",
                statusCode: StatusCodes.Status404NotFound
            );

        RegistrationEntity registrationEntity = registration.AsEntity(eventId);

        await registrationsRepository.Save(registrationEntity, cancellationToken);

        registration.Id = registrationEntity.Id.ToString();
        registration.EventId = registrationEntity.EventId.ToString();

        return TypedResults.Created
        (
            (string?)null,
            registration
        );
    }

    public static IEndpointRouteBuilder RegisterEventRegitrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(GetAllEventRegistrationsRoute, GetAllEventRegistrations)
            .RequireAuthorization()
            .WithDescription("Gets all registrations of an event with given ID.")
            .WithTags("Events")
            .Produces<IList<RegistrationResource>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)
            .Produces(StatusCodes.Status401Unauthorized);

        endpoints.MapPost(AddEventRegistrationRoute, AddEventRegistration)
            .WithDescription("Adds a new registration to the event with given ID.")
            .WithTags("Events")
            .Accepts<RegistrationResource>(MediaTypeNames.Application.Json)
            .Produces<RegistrationResource>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)
            .ProducesProblem(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson);

        return endpoints;
    }
}