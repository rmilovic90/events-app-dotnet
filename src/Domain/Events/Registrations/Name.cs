namespace Events.Domain.Events.Registrations;

public sealed record Name
{
    public const int MaxLength = 100;

    private string Value { get; }

    public Name(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfLongerThan(value, MaxLength);

        Value = value;
    }

    public override string ToString() => Value;
}