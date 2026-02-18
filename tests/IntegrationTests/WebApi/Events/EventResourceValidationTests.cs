using System.ComponentModel.DataAnnotations;

using Events.Domain.Events;

namespace Events.WebApi.Events;

public sealed class EventResourceValidationTests
{
    private static readonly DateTime UtcTomorrow = DateTime.UtcNow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private static Event ValidEvent => new()
    {
        Name = "Test",
        Description = "The test event.",
        Location = "Novi Sad, Serbia",
        StartTime = new DateTimeOffset(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 14, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)),
        EndTime = new DateTimeOffset(UtcTomorrow.Year, UtcTomorrow.Month, UtcTomorrow.Day, 15, 0, 0, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow))
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
        Assert.Single(validationResult.MemberNames, nameof(Event.Name));
    }

    [Fact]
    public void Validation_Fails_WhenNameIsTooLong()
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Name = new string('*', Name.MaxLength + 1);

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
        Assert.Single(validationResult.MemberNames, nameof(Event.Name));
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
        Assert.Single(validationResult.MemberNames, nameof(Event.Description));
    }

    [Fact]
    public void Validation_Fails_WhenDescriptionIsTooLong()
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Description = new string('*', Description.MaxLength + 1);

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
        Assert.Single(validationResult.MemberNames, nameof(Event.Description));
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
        Assert.Single(validationResult.MemberNames, nameof(Event.Location));
    }

    [Fact]
    public void Validation_Fails_WhenLocationIsTooLong()
    {
        Event invalidEvent = ValidEvent;
        invalidEvent.Location = new string('*', Location.MaxLength + 1);

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
        Assert.Single(validationResult.MemberNames, nameof(Event.Location));
    }

    [Fact]
    public void Validation_Fails_WhenStartTimeIsNotInTheFuture()
    {
        DateTime utcNow = DateTime.UtcNow;
        Event invalidEvent = ValidEvent;
        invalidEvent.StartTime = new DateTimeOffset(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)).AddHours(-1);

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
        Assert.Single(validationResult.MemberNames, nameof(Event.StartTime));
    }

    [Fact]
    public void Validation_Fails_WhenEndTimeIsNotAfterStartTime()
    {
        DateTime utcNow = DateTime.UtcNow;
        Event invalidEvent = ValidEvent;
        invalidEvent.StartTime = new DateTimeOffset(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)).AddDays(1);
        invalidEvent.EndTime = new DateTimeOffset(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow));

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
        Assert.Single(validationResult.MemberNames, nameof(Event.EndTime));
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