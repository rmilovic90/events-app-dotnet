using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Events.WebApi.Authentication;

public sealed class TokenRequest
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public sealed class Token
{
    public static readonly int DefaultExpirationInSeconds = 1800;

    public const string BearerTokenType = "Bearer";

    internal static Token BearerWithDefaultExpiration(string value) =>
        new()
        {
            AccessToken = value,
            TokenType = BearerTokenType,
            ExpiresIn = DefaultExpirationInSeconds
        };

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;

    [JsonPropertyName("expires_in")]
    [Description("Token validity period in seconds.")]
    public int ExpiresIn { get; set; }
}