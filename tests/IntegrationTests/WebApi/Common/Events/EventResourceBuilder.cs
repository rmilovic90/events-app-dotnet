using Events.WebApi.Events;

namespace Events.WebApi.Common.Events;

public sealed class EventResourceBuilder
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset UtcTomorrow = UtcNow.AddDays(1);
    private static readonly DateTimeOffset UtcDayAfterTomorrow = UtcTomorrow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private const string DefaultName = "Test";
    private const string DefaultDescription = "Test event.";
    private const string DefaultLocation = "Novi Sad, Serbia";
    private static readonly DateTimeOffset DefaultStartTime = new
    (
        UtcTomorrow.Year,
        UtcTomorrow.Month,
        UtcTomorrow.Day,
        UtcTomorrow.Hour,
        UtcTomorrow.Minute,
        UtcTomorrow.Second,
        CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)
    );
    private static readonly DateTimeOffset DefaultEndTime = new
    (
        UtcDayAfterTomorrow.Year,
        UtcDayAfterTomorrow.Month,
        UtcDayAfterTomorrow.Day,
        UtcDayAfterTomorrow.Hour,
        UtcDayAfterTomorrow.Minute,
        UtcDayAfterTomorrow.Second,
        CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)
    );

    public static EventResourceBuilder AnEventResource => new();

    private readonly string? _id = null;
    private string? _name = DefaultName;
    private string? _description = DefaultDescription;
    private string? _location = DefaultLocation;
    private DateTimeOffset _startTime = DefaultStartTime;
    private DateTimeOffset _endTime = DefaultEndTime;

    private EventResourceBuilder() { }

    public EventResourceBuilder WithName(string? value)
    {
        _name = value;

        return this;
    }

    public EventResourceBuilder WithoutName()
    {
        _name = null;

        return this;
    }

    public EventResourceBuilder WithDescription(string? value)
    {
        _description = value;

        return this;
    }

    public EventResourceBuilder WithoutDescription()
    {
        _description = null;

        return this;
    }

    public EventResourceBuilder WithLocation(string? value)
    {
        _location = value;

        return this;
    }

    public EventResourceBuilder WithoutLocation()
    {
        _location = null;

        return this;
    }

    public EventResourceBuilder WithStartTime(DateTimeOffset value)
    {
        _startTime = value;

        return this;
    }

    public EventResourceBuilder WithEndTime(DateTimeOffset value)
    {
        _endTime = value;

        return this;
    }

    public Event Build() =>
        new()
        {
            Id = _id,
            Name = _name!,
            Description = _description!,
            Location = _location!,
            StartTime = _startTime,
            EndTime = _endTime
        };
}