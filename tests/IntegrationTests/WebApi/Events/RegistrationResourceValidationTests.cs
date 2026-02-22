using System.ComponentModel.DataAnnotations;

using Events.Domain.Events;

using static Events.WebApi.Common.Events.Registrations.RegistrationResourceBuilder;

namespace Events.WebApi.Events;

public sealed class RegistrationResourceValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
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
            .WithName(new string('*', RegistrationName.MaxLength + 1))
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
    [InlineData("")]
    [InlineData("  ")]
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
            .WithPhoneNumber("+381aa")
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
    [InlineData("")]
    [InlineData("  ")]
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
            .WithEmailAddress($"{new string('*', RegistrationEmailAddress.MaxLength + 1)}@email.com")
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
            .WithEmailAddress("email")
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