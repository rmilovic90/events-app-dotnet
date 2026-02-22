using Events.WebApi.Authentication;
using Events.WebApi.Events;
using Events.WebApi.Events.Registrations;

namespace Events.WebApi;

internal static class Endpoints
{
    internal const string ApiBaseRoute = "api";

    internal static void RegisterAllEndpoints(this IEndpointRouteBuilder endpoints) =>
        endpoints.RegisterAuthenticationEndpoints()
            .RegisterEventsEndpoints()
            .RegisterEventRegitrationEndpoints();
}