namespace Events.Domain.Events;

public sealed record RegistrationEmailAddress
{
    private const string AtCharacter = "@";
    public const int MaxLength = 254;

    private string Value { get; }

    public RegistrationEmailAddress(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfLongerThan(value, MaxLength);
        ArgumentException.ThrowIfUnfulfilled
        (
            value,
            stringValue =>
            {
                if (stringValue.AsSpan().ContainsAny(' ', '\r', '\n')) return false;

                int index = stringValue.IndexOf(AtCharacter);

                return index > 0 &&
                    index != stringValue.Length - 1 &&
                    index == stringValue.LastIndexOf(AtCharacter);
            },
            $"Must contain exactly one '{AtCharacter}' which is neither the first nor the last character."
        );

        Value = value.Trim();
    }

    public override string ToString() => Value;
}