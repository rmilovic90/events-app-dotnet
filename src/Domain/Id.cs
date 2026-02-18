namespace Events.Domain;

public sealed record Id
{
    private string Value { get; }

    public Id() => Value = Guid.CreateVersion7().ToString();

    public Id(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
    }

    public override string ToString() => Value;
}