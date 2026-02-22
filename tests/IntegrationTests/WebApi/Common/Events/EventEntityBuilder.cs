using Events.Domain;
using Events.Domain.Events;

namespace Events.WebApi.Common.Events;

public sealed class EventEntityBuilder
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset UtcTomorrow = UtcNow.AddDays(1);
    private static readonly DateTimeOffset UtcDayAfterTomorrow = UtcTomorrow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private static readonly Name DefaultName = new("Test");
    private static readonly Description DefaultDescription = new("Test event.");
    private static readonly Location DefaultLocation = new("Novi Sad, Serbia");
    private static readonly StartTime DefaultStartTime = StartTime.Of
    (
        new DateTimeOffset
        (
            UtcTomorrow.Year,
            UtcTomorrow.Month,
            UtcTomorrow.Day,
            UtcTomorrow.Hour,
            UtcTomorrow.Minute,
            UtcTomorrow.Second,
            CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)
        )
    );
    private static readonly EndTime DefaultEndTime = EndTime.Of
    (
        new DateTimeOffset
        (
            UtcDayAfterTomorrow.Year,
            UtcDayAfterTomorrow.Month,
            UtcDayAfterTomorrow.Day,
            UtcDayAfterTomorrow.Hour,
            UtcDayAfterTomorrow.Minute,
            UtcDayAfterTomorrow.Second,
            CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)
        )
    );

    public static EventEntityBuilder ANewEventEntity => new();

    private Id _id = new();
    private Name _name = DefaultName;
    private Description _description = DefaultDescription;
    private Location _location = DefaultLocation;
    private readonly StartTime _startTime = DefaultStartTime;
    private readonly EndTime _endTime = DefaultEndTime;

    public EventEntityBuilder WithId(string value)
    {
        _id = new(value);

        return this;
    }

    public EventEntityBuilder WithName(string value)
    {
        _name = new(value);

        return this;
    }

    public EventEntityBuilder WithDescription(string value)
    {
        _description = new(value);

        return this;
    }

    public EventEntityBuilder WithLocation(string value)
    {
        _location = new(value);

        return this;
    }

    private EventEntityBuilder() { }

    public Event Build() =>
        Event.Of
        (
            _id,
            _name,
            _description,
            _location,
            _startTime,
            _endTime
        );
}