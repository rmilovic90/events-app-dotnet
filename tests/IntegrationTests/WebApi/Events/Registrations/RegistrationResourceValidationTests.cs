using System.ComponentModel.DataAnnotations;

using static Events.WebApi.Events.Registrations.RegistrationResourceBuilder;

namespace Events.WebApi.Events.Registrations;

public sealed class RegistrationResourceValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(AnEmptyRegistrationName)]
    [InlineData(RegistrationNameWithWhitespacesOnly)]
    public void Validation_Fails_WhenNameIsMissing(string? name)
    {
        Registration invalidEventRegistration = ARegistrationResource
            .WithName(name)
            .Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEventRegistration,
            new ValidationContext(invalidEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(Registration.Name));
    }

    [Fact]
    public void Validation_Fails_WhenNameIsTooLong()
    {
        Registration invalidEventRegistration = ARegistrationResource
            .WithName(TooLongRegistrationName)
            .Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEventRegistration,
            new ValidationContext(invalidEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(Registration.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(AnEmptyRegistrationPhoneNumber)]
    [InlineData(RegistrationPhoneNumberWithWhitespacesOnly)]
    public void Validation_Fails_WhenPhoneNumberIsMissing(string? phoneNumber)
    {
        Registration invalidEventRegistration = ARegistrationResource
            .WithPhoneNumber(phoneNumber)
            .Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEventRegistration,
            new ValidationContext(invalidEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(Registration.PhoneNumber));
    }

    [Fact]
    public void Validation_Fails_WhenPhoneNumberHasInvalidFormat()
    {
        Registration invalidEventRegistration = ARegistrationResource
            .WithPhoneNumber(RegistrationPhoneNumberWithInvalidFormat)
            .Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEventRegistration,
            new ValidationContext(invalidEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(Registration.PhoneNumber));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(AnEmptyRegistrationEmailAddress)]
    [InlineData(RegistrationEmailAddressWithWhitespacesOnly)]
    public void Validation_Fails_WhenEmailAddressIsMissing(string? emailAddress)
    {
        Registration invalidEventRegistration = ARegistrationResource
            .WithEmailAddress(emailAddress)
            .Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEventRegistration,
            new ValidationContext(invalidEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(Registration.EmailAddress));
    }

    [Fact]
    public void Validation_Fails_WhenEmailAddressIsTooLong()
    {
        Registration invalidEventRegistration = ARegistrationResource
            .WithEmailAddress(TooLongRegistrationEmailAddress)
            .Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEventRegistration,
            new ValidationContext(invalidEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(Registration.EmailAddress));
    }

    [Fact]
    public void Validation_Fails_WhenEmailAddressHasInvalidFormat()
    {
        Registration invalidEventRegistration = ARegistrationResource
            .WithEmailAddress(RegistrationEmailAddressWithInvalidFormat)
            .Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEventRegistration,
            new ValidationContext(invalidEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(Registration.EmailAddress));
    }

    [Fact]
    public void Validation_Succeeds_WhenEventResourceIsValid()
    {
        Registration validEventRegistration = ARegistrationResource.Build();

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            validEventRegistration,
            new ValidationContext(validEventRegistration),
            validationResults,
            validateAllProperties: true
        );

        Assert.True(isValid);
    }
}