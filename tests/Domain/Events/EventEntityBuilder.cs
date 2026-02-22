namespace Events.Domain.Events;

public sealed class EventEntityBuilder
{
    public const string AnEventIdValue = "019c770f-52d0-7656-9298-adeecf45987a";
    public const string AnEmptyEventNameValue = "";
    public const string EventNameValueWithWhitespacesOnly = "  ";
    public static readonly string TooLongEventNameValue = new('*', Name.MaxLength + 1);
    public const string AnEventNameValue = "Test";
    public const string AnEmptyEventDescriptionValue = "";
    public const string EventDescriptionValueWithWhitespacesOnly = "  ";
    public static readonly string TooLongEventDescriptionValue = new('*', Description.MaxLength + 1);
    public const string AnEventDescriptionValue = "Test event.";
    public const string AnEmptyEventLocationValue = "";
    public const string EventLocationValueWithWhitespacesOnly = "  ";
    public static readonly string TooLongEventLocationValue = new('*', Location.MaxLength + 1);
    public const string AnEventLocationValue = "Novi Sad, Serbia";

    private static readonly DateTimeOffset Today = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset Yesterday = Today.AddDays(-1);
    private static readonly DateTimeOffset Tomorrow = Today.AddDays(1);
    private static readonly DateTimeOffset FollowingDay = Tomorrow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    public static readonly DateTimeOffset TodayEventTimeValue = new
    (
        Today.Year,
        Today.Month,
        Today.Day,
        Today.Hour,
        Today.Minute,
        Today.Second,
        CentralEuropeanTimeZone.GetUtcOffset(Today)
    );
    public static readonly DateTimeOffset YesterdayEventTimeValue = new
    (
        Yesterday.Year,
        Yesterday.Month,
        Yesterday.Day,
        Yesterday.Hour,
        Yesterday.Minute,
        Yesterday.Second,
        CentralEuropeanTimeZone.GetUtcOffset(Yesterday)
    );
    public static readonly DateTimeOffset TomorrowEventTimeValue = new
    (
        Tomorrow.Year,
        Tomorrow.Month,
        Tomorrow.Day,
        Tomorrow.Hour,
        Tomorrow.Minute,
        Tomorrow.Second,
        CentralEuropeanTimeZone.GetUtcOffset(Tomorrow)
    );
    public static readonly DateTimeOffset FollowingDayEventTimeValue = new
    (
        FollowingDay.Year,
        FollowingDay.Month,
        FollowingDay.Day,
        FollowingDay.Hour,
        FollowingDay.Minute,
        FollowingDay.Second,
        CentralEuropeanTimeZone.GetUtcOffset(FollowingDay)
    );

    public static readonly Id AnEventId = new();
    public static readonly Name AnEventName = new(AnEventNameValue);
    public static readonly Description AnEventDescription = new(AnEventDescriptionValue);
    public static readonly Location AnEventLocation = new(AnEventLocationValue);
    public static readonly StartTime TomorrowStartTime = StartTime.Of(TomorrowEventTimeValue);
    public static readonly EndTime FollowingDayEndTime = EndTime.Of(FollowingDayEventTimeValue);

    public static EventEntityBuilder ANewEventEntity => new(new Id());

    private Id _id = AnEventId;
    private Name _name = AnEventName;
    private Description _description = AnEventDescription;
    private Location _location = AnEventLocation;
    private readonly StartTime _startTime = TomorrowStartTime;
    private readonly EndTime _endTime = FollowingDayEndTime;

    private EventEntityBuilder(Id id) => _id = id;

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