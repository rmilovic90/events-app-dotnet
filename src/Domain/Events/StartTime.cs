namespace Events.Domain.Events;

public sealed record StartTime
{
    public static StartTime New(DateTimeOffset value, TimeProvider timeProvider)
    {
        ArgumentException.ThrowIfUnfulfilled
        (
            value,
            dateTimeOffsetValue => dateTimeOffsetValue.ToUniversalTime() > timeProvider.GetUtcNow(),
            "Must be in the future."
        );

        return new(value);
    }

    public static StartTime Of(DateTimeOffset value) => new(value);

    public DateTimeOffset Value { get; }

    private StartTime(DateTimeOffset value) => Value = value;

    public override string ToString() => Value.ToString();
}