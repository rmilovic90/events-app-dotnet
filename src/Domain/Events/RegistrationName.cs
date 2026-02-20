namespace Events.Domain.Events;

public sealed record RegistrationName
{
    public const int MaxLength = 100;

    private string Value { get; }

    public RegistrationName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfLongerThan(value, MaxLength);

        Value = value;
    }

    public override string ToString() => Value;
}