using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Events.WebApi;

internal sealed class TestAuthHandler
(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>
(
    options,
    logger,
    encoder
)
{
    public const string TestAuthenticationScheme = "TestScheme";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
        Task.FromResult
        (
            AuthenticateResult.Success
            (
                new AuthenticationTicket
                (
                    new ClaimsPrincipal
                    (
                        new ClaimsIdentity
                        (
                            [new Claim(ClaimTypes.Name, "Test user")],
                            "Test"
                        )
                    ),
                    TestAuthenticationScheme
                )
            )
        );
}