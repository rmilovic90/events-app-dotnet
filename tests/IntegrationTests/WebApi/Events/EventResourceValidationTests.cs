using System.ComponentModel.DataAnnotations;

using Events.Domain.Events;

using static Events.WebApi.Common.Events.EventResourceBuilder;

namespace Events.WebApi.Events;

public sealed class EventResourceValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
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
            .WithName(new string('*', Name.MaxLength + 1))
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
    [InlineData("")]
    [InlineData("  ")]
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
            .WithDescription(new string('*', Description.MaxLength + 1))
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
    [InlineData("")]
    [InlineData("  ")]
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
            .WithLocation(new string('*', Location.MaxLength + 1))
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
            .WithStartTime(DateTimeOffset.UtcNow.AddDays(-1))
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
            .WithStartTime(DateTimeOffset.UtcNow.AddDays(1))
            .WithEndTime(DateTimeOffset.UtcNow)
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