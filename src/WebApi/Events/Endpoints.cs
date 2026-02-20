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
    internal const string CreateRoute = BaseRoute;
    internal const string AddRegistrationRoute = $"{BaseRoute}/{{id}}/registrations";

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
                title: $"Event with ID {eventId} does not exist",
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
        endpoints.MapPost(CreateRoute, Create)
            .RequireAuthorization()
            .WithDescription("Creates a new event.")
            .WithTags("Events")
            .Produces(StatusCodes.Status201Created, responseType: typeof(EventResource), contentType: MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
            .Produces(StatusCodes.Status401Unauthorized);

        endpoints.MapPost(AddRegistrationRoute, AddRegistration)
            .WithDescription("Adds a new registration to the event.")
            .WithTags("Events")
            .Produces(StatusCodes.Status201Created, responseType: typeof(RegistrationResource), contentType: MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
            .ProducesProblem(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.ProblemJson);


        return endpoints;
    }
}