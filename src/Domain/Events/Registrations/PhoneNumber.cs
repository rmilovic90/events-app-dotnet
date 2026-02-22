using System.Text.RegularExpressions;

namespace Events.Domain.Events.Registrations;

public sealed partial record PhoneNumber
{
    public const string FormatPattern = "^\\+[1-9]\\d{1,14}$";

    private string Value { get; }

    public PhoneNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfNotMatched(value, FormatRegex());

        Value = value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(FormatPattern)]
    private static partial Regex FormatRegex();
}