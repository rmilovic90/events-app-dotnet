using Events.WebApi.Events;

namespace Events.WebApi;

internal static class Endpoints
{
    internal const string ApiBaseRoute = "api";

    public static void RegisterAllEndpoints(this IEndpointRouteBuilder endpoints) =>
        endpoints.RegisterEventsEndpoints();
}