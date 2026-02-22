using Events.Domain.Events.Registrations;

namespace Events.WebApi.Events.Registrations;

public sealed class RegistrationResourceBuilder
{
    public const string AnEmptyRegistrationName = "";
    public const string RegistrationNameWithWhitespacesOnly = "  ";
    public static readonly string TooLongRegistrationName = new('*', Name.MaxLength + 1);
    public const string ARegistrationName = "Jane Doe";
    public const string AnEmptyRegistrationPhoneNumber = "";
    public const string RegistrationPhoneNumberWithWhitespacesOnly = "  ";
    public const string RegistrationPhoneNumberWithInvalidFormat = "+381aa";
    public const string ARegistrationPhoneNumber = "+38155555555";
    public const string AnEmptyRegistrationEmailAddress = "";
    public const string RegistrationEmailAddressWithWhitespacesOnly = "  ";
    public static readonly string TooLongRegistrationEmailAddress = $"{new string('*', EmailAddress.MaxLength + 1)}@email.com";
    public const string RegistrationEmailAddressWithInvalidFormat = "email.com";
    public const string ARegistrationEmailAddress = "jane.doe@email.com";

    public static RegistrationResourceBuilder ARegistrationResource => new();

    private readonly string? _id = null;
    private readonly string? _eventId = null;
    private string _name = ARegistrationName;
    private string _phoneNumber = ARegistrationPhoneNumber;
    private string _emailAddress = ARegistrationEmailAddress;

    private RegistrationResourceBuilder() { }

    public RegistrationResourceBuilder WithName(string? value)
    {
        _name = value!;

        return this;
    }

    public RegistrationResourceBuilder WithPhoneNumber(string? value)
    {
        _phoneNumber = value!;

        return this;
    }

    public RegistrationResourceBuilder WithEmailAddress(string? value)
    {
        _emailAddress = value!;

        return this;
    }

    public Registration Build() =>
        new()
        {
            Id = _id,
            EventId = _eventId,
            Name = _name,
            PhoneNumber = _phoneNumber,
            EmailAddress = _emailAddress
        };
}