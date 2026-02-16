using Events.WebApi.Authentication;
using Events.WebApi.Events;

namespace Events.WebApi;

internal static class Endpoints
{
    internal const string ApiBaseRoute = "api";

    internal static void RegisterAllEndpoints(this IEndpointRouteBuilder endpoints) =>
        endpoints.RegisterAuthenticationEndpoints()
            .RegisterEventsEndpoints();
}