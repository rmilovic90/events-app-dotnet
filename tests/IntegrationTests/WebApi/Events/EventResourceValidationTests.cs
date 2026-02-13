using System.ComponentModel.DataAnnotations;

namespace Events.WebApi.Events;

public sealed class EventResourceValidationTests
{
    private static Event ValidEvent => new()
    {
        Name = "Test",
        Description = "The test event.",
        Location = "Novi Sad, Serbia",
        StartTime = DateTime.UtcNow.Date.AddDays(2),
        EndTime = DateTime.UtcNow
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validation_Fails_WhenNameIsMissing(string? name)
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Name = name!;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEvent,
            new ValidationContext(invalidEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(ValidEvent.Name));
    }

    [Fact]
    public void Validation_Fails_WhenNameIsTooLong()
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Name = new string('*', Event.MaxAllowedNameLength + 1);

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEvent,
            new ValidationContext(invalidEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(ValidEvent.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validation_Fails_WhenDescriptionIsMissing(string? description)
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Description = description!;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEvent,
            new ValidationContext(invalidEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(ValidEvent.Description));
    }

    [Fact]
    public void Validation_Fails_WhenDescriptionIsTooLong()
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Description = new string('*', Event.MaxAllowedDescriptionLength + 1);

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEvent,
            new ValidationContext(invalidEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(ValidEvent.Description));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validation_Fails_WhenLocationIsMissing(string? location)
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Location = location!;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEvent,
            new ValidationContext(invalidEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(ValidEvent.Location));
    }

    [Fact]
    public void Validation_Fails_WhenLocationIsTooLong()
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Location = new string('*', Event.MaxAllowedLocationLength + 1);

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEvent,
            new ValidationContext(invalidEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(ValidEvent.Location));
    }

    [Fact]
    public void Validation_Fails_WhenStartTimeIsNotAtLeastOneDayInTheFuture()
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.StartTime = DateTime.UtcNow;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            invalidEvent,
            new ValidationContext(invalidEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        ValidationResult validationResult = Assert.Single(validationResults);
        Assert.Single(validationResult.MemberNames, nameof(ValidEvent.StartTime));
    }

    [Fact]
    public void Validation_Succeeds_WhenEventResourceIsValid()
    {
        Event validEvent = ValidEvent;

        List<ValidationResult> validationResults = [];

        bool isValid = Validator.TryValidateObject
        (
            validEvent,
            new ValidationContext(validEvent),
            validationResults,
            validateAllProperties: true
        );

        Assert.True(isValid);
    }
}