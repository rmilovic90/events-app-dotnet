namespace Events.WebApi.Authentication;

public sealed class TokenRequestResourceBuilder
{
    public const string AnEmptyTokenRequestUsername = "";
    public const string TokenRequestUsernameWithWhitespacesOnly = "  ";
    private const string ATokenRequestUsername = "user";
    public const string AnEmptyTokenRequestPassword = "";
    public const string TokenRequestPasswordWithWhitespacesOnly = "  ";
    private const string ATokenRequestPassword = "password";

    public static TokenRequestResourceBuilder ATokenRequestResource => new();

    private string _username = ATokenRequestUsername;
    private string _password = ATokenRequestPassword;

    private TokenRequestResourceBuilder() { }

    public TokenRequestResourceBuilder WithUsername(string? value)
    {
        _username = value!;

        return this;
    }

    public TokenRequestResourceBuilder WithPassword(string? value)
    {
        _password = value!;

        return this;
    }

    public TokenRequest Build() =>
        new()
        {
            Username = _username,
            Password = _password
        };
}