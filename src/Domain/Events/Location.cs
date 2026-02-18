namespace Events.Domain.Events;

public sealed record Location
{
    public const int MaxLength = 100;

    private string Value { get; }

    public Location(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfLongerThan(value, MaxLength);

        Value = value;
    }

    public override string ToString() => Value;
}