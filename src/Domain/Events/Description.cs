namespace Events.Domain.Events;

public sealed record Description
{
    public const int MaxLength = 200;

    private string Value { get; }

    public Description(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfLongerThan(value, MaxLength);

        Value = value;
    }

    public override string ToString() => Value;
}