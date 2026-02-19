using System.Net.Mime;

using Events.Domain.Events;

using EventEntity = Events.Domain.Events.Event;
using EventResource = Events.WebApi.Events.Event;
using RootEndpoints = Events.WebApi.Endpoints;

namespace Events.WebApi.Events;

internal static class Endpoints
{
    private const string BaseRoute = $"{RootEndpoints.ApiBaseRoute}/events";
    internal const string CreateRoute = BaseRoute;

    private static async Task<IResult> Create
    (
        EventResource @event,
        IEventsRepository repository,
        CancellationToken cancellationToken
    )
    {
        EventEntity entity = @event.AsEntity();

        await repository.Save(@event.AsEntity(), cancellationToken);

        return TypedResults.Created
        (
            (string?)null,
            EventResource.FromEntity(entity)
        );
    }

    public static IEndpointRouteBuilder RegisterEventsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(CreateRoute, Create)
            .RequireAuthorization()
            .WithDescription("Creates a new event.")
            .WithTags("Events")
            .Produces(StatusCodes.Status201Created, responseType: typeof(EventResource), contentType: MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson)
            .Produces(StatusCodes.Status401Unauthorized);

        return endpoints;
    }
}