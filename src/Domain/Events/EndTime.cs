namespace Events.Domain.Events;

public sealed record EndTime
{
    public static EndTime Of(DateTimeOffset value, StartTime startTime)
    {
        ArgumentException.ThrowIfUnfulfilled
        (
            value,
            dateTimeOffsetValue => dateTimeOffsetValue > startTime.Value,
            $"Must be after {nameof(startTime)}."
        );

        return new(value);
    }

    public DateTimeOffset Value { get; }

    private EndTime(DateTimeOffset value) => Value = value;

    public override string ToString() => Value.ToString();
}