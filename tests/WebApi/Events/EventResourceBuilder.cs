using Events.Domain.Events;

namespace Events.WebApi.Events;

public sealed class EventResourceBuilder
{
    public const string AnEmptyEventName = "";
    public const string EventNameWithWhitespacesOnly = "  ";
    public static readonly string TooLongEventName = new('*', Name.MaxLength + 1);
    public const string AnEventName = "Test";
    public const string AnEmptyEventDescription = "";
    public const string EventDescriptionWithWhitespacesOnly = "  ";
    public static readonly string TooLongEventDescription = new('*', Description.MaxLength + 1);
    public const string AnEventDescription = "Test event.";
    public const string AnEmptyEventLocation = "";
    public const string EventLocationWithWhitespacesOnly = "  ";
    public static readonly string TooLongEventLocation = new('*', Location.MaxLength + 1);
    private const string AnEventLocation = "Novi Sad, Serbia";

    private static readonly DateTimeOffset Today = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset Yesterday = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset Tomorrow = Today.AddDays(1);
    private static readonly DateTimeOffset FollowingDay = Tomorrow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    public static readonly DateTimeOffset TodayEventTime = new
    (
        Today.Year,
        Today.Month,
        Today.Day,
        Today.Hour,
        Today.Minute,
        Today.Second,
        CentralEuropeanTimeZone.GetUtcOffset(Today)
    );
    public static readonly DateTimeOffset YesterdayEventTime = new
    (
        Yesterday.Year,
        Yesterday.Month,
        Yesterday.Day,
        Yesterday.Hour,
        Yesterday.Minute,
        Yesterday.Second,
        CentralEuropeanTimeZone.GetUtcOffset(Yesterday)
    );
    public static readonly DateTimeOffset TomorrowEventTime = new
    (
        Tomorrow.Year,
        Tomorrow.Month,
        Tomorrow.Day,
        Tomorrow.Hour,
        Tomorrow.Minute,
        Tomorrow.Second,
        CentralEuropeanTimeZone.GetUtcOffset(Tomorrow)
    );
    private static readonly DateTimeOffset FollowingDayEventTime = new
    (
        FollowingDay.Year,
        FollowingDay.Month,
        FollowingDay.Day,
        FollowingDay.Hour,
        FollowingDay.Minute,
        FollowingDay.Second,
        CentralEuropeanTimeZone.GetUtcOffset(Tomorrow)
    );

    public static EventResourceBuilder AnEventResource => new();

    private readonly string? _id = null;
    private string? _name = AnEventName;
    private string? _description = AnEventDescription;
    private string? _location = AnEventLocation;
    private DateTimeOffset _startTime = TomorrowEventTime;
    private DateTimeOffset _endTime = FollowingDayEventTime;

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