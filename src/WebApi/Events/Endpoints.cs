using System.Net.Mime;

using Events.Domain;
using Events.Domain.Events;

using Microsoft.AspNetCore.Mvc;

using EventEntity = Events.Domain.Events.Event;
using EventResource = Events.WebApi.Events.Event;
using RootEndpoints = Events.WebApi.Endpoints;

namespace Events.WebApi.Events;

internal static class Endpoints
{
    private const string BaseRoute = $"{RootEndpoints.ApiBaseRoute}/events";
    internal const string GetAllRoute = BaseRoute;
    internal const string GetSingleRoute = $"{BaseRoute}/{{id}}";
    internal const string CreateRoute = BaseRoute;

    private static async Task<IResult> GetAll
    (
        IRepository repository,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<EventEntity> events = await repository.GetAll(cancellationToken);

        return TypedResults.Ok(events.Select(EventResource.FromEntity));
    }

    private static async Task<IResult> GetSingle
    (
        [FromRoute] string id,
        IRepository repository,
        CancellationToken cancellationToken
    )
    {
        EventEntity? @event = await repository.Get(new Id(id), cancellationToken);

        return @event is null
            ? TypedResults.Problem
            (
                title: $"Event with ID '{id}' does not exist.",
                statusCode: StatusCodes.Status404NotFound
            )
            : TypedResults.Ok(EventResource.FromEntity(@event));
    }

    private static async Task<IResult> Create
    (
        EventResource @event,
        IRepository repository,
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

        return endpoints;
    }
}