using System.Net.Mime;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Mvc;

using EventEntity = Events.Domain.Events.Event;
using EventResource = Events.WebApi.Events.Event;
using RegistrationEntity = Events.Domain.Events.Registration;
using RegistrationResource = Events.WebApi.Events.Registration;
using RootEndpoints = Events.WebApi.Endpoints;

namespace Events.WebApi.Events;

internal static class Endpoints
{
    private const string BaseRoute = $"{RootEndpoints.ApiBaseRoute}/events";
    internal const string GetAllRoute = BaseRoute;
    internal const string GetSingleRoute = $"{BaseRoute}/{{id}}";
    internal const string CreateRoute = BaseRoute;
    internal const string AddRegistrationRoute = $"{GetSingleRoute}/registrations";

    private static async Task<IResult> GetAll
    (
        IEventsRepository repository,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<EventEntity> events = await repository.GetAll(cancellationToken);

        return TypedResults.Ok(events.Select(EventResource.FromEntity));
    }

    private static async Task<IResult> GetSingle
    (
        [FromRoute] string id,
        IEventsRepository repository,
        CancellationToken cancellationToken
    )
    {
        EventEntity? @event = await repository.Get(new Id(id), cancellationToken);

        return @event is null
            ? TypedResults.Problem
            (
                title: $"Event with ID '{id}' does not exist",
                statusCode: StatusCodes.Status404NotFound
            )
            : TypedResults.Ok(EventResource.FromEntity(@event));
    }

    private static async Task<IResult> Create
    (
        EventResource @event,
        IEventsRepository repository,
        CancellationToken cancellationToken
    )
    {
        EventEntity entity = @event.AsEntity();

        await repository.Save(entity, cancellationToken);

        return TypedResults.Created
        (
            (string?)null,
            EventResource.FromEntity(entity)
        );
    }

    private static async Task<IResult> AddRegistration
    (
        [FromRoute(Name = "id")] string eventId,
        [FromBody] RegistrationResource registration,
        IEventsRepository repository,
        CancellationToken cancellationToken
    )
    {
        EventEntity? @event = await repository.Get(new Id(eventId), cancellationToken);

        if (@event is null)
            return TypedResults.Problem
            (
                title: $"Event with ID '{eventId}' does not exist",
                statusCode: StatusCodes.Status404NotFound
            );

        RegistrationEntity registrationEntity = registration.AsEntity(eventId);
        @event.Add(registrationEntity);

        await repository.Save(@event, cancellationToken);

        registration.Id = registrationEntity.Id.ToString();
        registration.EventId = registrationEntity.EventId.ToString();

        return TypedResults.Created
        (
            (string?)null,
            registration
        );
    }

    public static IEndpointRouteBuilder RegisterEventsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(GetAllRoute, GetAll)
            .WithDescription("Gets all events.")
            .WithTags("Events")
            .Produces<IList<EventResource>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);

        endpoints.MapGet(GetSingleRoute, GetSingle)
            .WithDescription("Gets single event by its ID.")
            .WithTags("Events")
            .Produces<EventResource>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson);

        endpoints.MapPost(CreateRoute, Create)
            .RequireAuthorization()
            .WithDescription("Creates a new event.")
            .WithTags("Events")
            .Accepts<EventResource>(MediaTypeNames.Application.Json)
            .Produces<EventResource>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)
            .Produces(StatusCodes.Status401Unauthorized);

        endpoints.MapPost(AddRegistrationRoute, AddRegistration)
            .WithDescription("Adds a new registration to the event with given ID.")
            .WithTags("Events")
            .Accepts<RegistrationResource>(MediaTypeNames.Application.Json)
            .Produces<RegistrationResource>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)
            .ProducesProblem(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson);

        return endpoints;
    }
}