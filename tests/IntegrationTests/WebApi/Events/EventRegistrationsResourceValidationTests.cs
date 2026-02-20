using System.ComponentModel.DataAnnotations;

using Events.Domain.Events;

namespace Events.WebApi.Events;

public sealed class EventRegistrationsResourceValidationTests
{
    private static Registration ValidEventRegistration => new()
    {
        Name = "Jane Doe",
        PhoneNumber = "+38155555555",
        EmailAddress = "jane.doe@email.com"
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validation_Fails_WhenNameIsMissing(string? name)
    {
        Registration invalidEventRegistration = ValidEventRegistration;
        invalidEventRegistration.Name = name!;

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
        Registration invalidEventRegistration = ValidEventRegistration;
        invalidEventRegistration.Name = new string('*', RegistrationName.MaxLength + 1);

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
        Registration invalidEventRegistration = ValidEventRegistration;
        invalidEventRegistration.PhoneNumber = phoneNumber!;

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
        Registration invalidEventRegistration = ValidEventRegistration;
        invalidEventRegistration.PhoneNumber = "+381aa";

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
        Registration invalidEventRegistration = ValidEventRegistration;
        invalidEventRegistration.EmailAddress = emailAddress!;

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
        Registration invalidEventRegistration = ValidEventRegistration;
        invalidEventRegistration.EmailAddress = $"{new string('*', RegistrationEmailAddress.MaxLength + 1)}@email.com";

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
        Registration invalidEventRegistration = ValidEventRegistration;
        invalidEventRegistration.EmailAddress = "email";

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
        Registration validEventRegistration = ValidEventRegistration;

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