using System.ComponentModel.DataAnnotations;

using static Events.WebApi.Authentication.TokenRequestResourceBuilder;

namespace Events.WebApi.Authentication;

public sealed class TokenRequestValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(AnEmptyTokenRequestUsername)]
    [InlineData(TokenRequestUsernameWithWhitespacesOnly)]
    public void Validation_Fails_WhenUsernameIsMissing(string? username)
    {
        TokenRequest invalidTokenRequest = ATokenRequestResource
            .WithUsername(username)
            .Build();

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
    [InlineData(AnEmptyTokenRequestPassword)]
    [InlineData(TokenRequestPasswordWithWhitespacesOnly)]
    public void Validation_Fails_WhenPasswordIsMissing(string? password)
    {
        TokenRequest invalidTokenRequest = ATokenRequestResource
            .WithPassword(password)
            .Build();

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
        TokenRequest validTokenRequest = ATokenRequestResource.Build();

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