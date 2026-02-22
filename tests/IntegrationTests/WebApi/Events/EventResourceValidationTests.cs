using System.ComponentModel.DataAnnotations;

using static Events.WebApi.Events.EventResourceBuilder;

namespace Events.WebApi.Events;

public sealed class EventResourceValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(AnEmptyEventName)]
    [InlineData(EventNameWithWhitespacesOnly)]
    public void Validation_Fails_WhenNameIsMissing(string? name)
    {
        Event invalidEvent = AnEventResource
            .WithName(name)
            .Build();

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
        Event invalidEvent = AnEventResource
            .WithName(TooLongEventName)
            .Build();

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
    [InlineData(AnEmptyEventDescription)]
    [InlineData(EventDescriptionWithWhitespacesOnly)]
    public void Validation_Fails_WhenDescriptionIsMissing(string? description)
    {
        Event invalidEvent = AnEventResource
            .WithDescription(description)
            .Build();

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
        Event invalidEvent = AnEventResource
            .WithDescription(TooLongEventDescription)
            .Build();

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
    [InlineData(AnEmptyEventLocation)]
    [InlineData(EventLocationWithWhitespacesOnly)]
    public void Validation_Fails_WhenLocationIsMissing(string? location)
    {
        Event invalidEvent = AnEventResource
            .WithLocation(location)
            .Build();

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
        Event invalidEvent = AnEventResource
            .WithLocation(TooLongEventLocation)
            .Build();

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
        Event invalidEvent = AnEventResource
            .WithStartTime(YesterdayEventTime)
            .Build();

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
        Event invalidEvent = AnEventResource
            .WithStartTime(TomorrowEventTime)
            .WithEndTime(TodayEventTime)
            .Build();

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
        Event validEvent = AnEventResource.Build();

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