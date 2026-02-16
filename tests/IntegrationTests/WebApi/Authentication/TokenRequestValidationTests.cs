using System.ComponentModel.DataAnnotations;

namespace Events.WebApi.Authentication;

public sealed class TokenRequestValidationTests
{
    private static TokenRequest ValidTokenRequest => new()
    {
        Username = "user",
        Password = "password"
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validation_Fails_WhenUsernameIsMissing(string? username)
    {
        TokenRequest invalidTokenRequest = ValidTokenRequest;
        invalidTokenRequest.Username = username!;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidTokenRequest,
            new ValidationContext(invalidTokenRequest),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(TokenRequest.Username));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validation_Fails_WhenPasswordIsMissing(string? password)
    {
        TokenRequest invalidTokenRequest = ValidTokenRequest;
        invalidTokenRequest.Password = password!;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidTokenRequest,
            new ValidationContext(invalidTokenRequest),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(TokenRequest.Password));
    }

    [Fact]
    public void Validation_Succeeds_WhenTokenRequestResourceIsValid()
    {
        TokenRequest validTokenRequest = ValidTokenRequest;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            validTokenRequest,
            new ValidationContext(validTokenRequest),
            validationResults,
            validateAllProperties: true
        );

        Assert.True(isValid);
    }
}