using System.Net.Mime;

using RootEndpoints = Events.WebApi.Endpoints;

namespace Events.WebApi.Events;

internal static class Endpoints
{
    private const string BaseRoute = $"{RootEndpoints.ApiBaseRoute}/events";
    internal const string CreateRoute = BaseRoute;

    private static IResult Create(Event @event) => TypedResults.Created();

    public static IEndpointRouteBuilder RegisterEventsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(CreateRoute, Create)
            .WithDescription("Creates a new event.")
            .WithTags("Events")
            .Produces(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.ProblemJson);

        return endpoints;
    }
}