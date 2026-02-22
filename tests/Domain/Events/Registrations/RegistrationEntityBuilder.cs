namespace Events.Domain.Events.Registrations;

public sealed class RegistrationEntityBuilder
{
    public const string AnEmptyRegistrationNameValue = "";
    public const string RegistrationNameValueWithWhitespacesOnly = "  ";
    public static readonly string TooLongRegistrationNameValue = new('*', Name.MaxLength + 1);
    private const string ARegistrationNameValue = "Jane Doe";
    public const string AnEmptyRegistrationPhoneNumberValue = "";
    public const string RegistrationPhoneNumberValueWithWhitespacesOnly = "  ";
    public const string ARegistrationPhoneNumberValue = "+38155555555";
    public const string AnEmptyRegistrationEmailAddressValue = "";
    public const string RegistrationEmailAddressValueWithWhitespacesOnly = "  ";
    public static readonly string TooLongRegistrationEmailAddressValue = $"{new('*', EmailAddress.MaxLength + 1)}@email.com";
    public const string ARegistrationEmailAddressValue = "jane.doe@email.com";

    public static readonly Id ARegistrationId = new();
    public static readonly Id ARegistrationEventId = new();
    public static readonly Name ARegistrationName = new(ARegistrationNameValue);
    public static readonly PhoneNumber ARegistrationPhoneNumber = new(ARegistrationPhoneNumberValue);
    public static readonly EmailAddress ARegistrationEmailAddress = new(ARegistrationEmailAddressValue);

    public static RegistrationEntityBuilder ANewRegistrationEntity => new(new Id());

    private readonly Id _id = ARegistrationId;
    private Id _eventId = ARegistrationEventId;
    private Name _name = ARegistrationName;
    private PhoneNumber _phoneNumber = ARegistrationPhoneNumber;
    private EmailAddress _emailAddress = ARegistrationEmailAddress;

    private RegistrationEntityBuilder(Id id) => _id = id;

    public RegistrationEntityBuilder WithEventId(string value)
    {
        _eventId = new(value);

        return this;
    }

    public RegistrationEntityBuilder WithEventId(Id value)
    {
        _eventId = value;

        return this;
    }

    public RegistrationEntityBuilder WithName(string value)
    {
        _name = new(value);

        return this;
    }

    public RegistrationEntityBuilder WithPhoneNumber(string value)
    {
        _phoneNumber = new(value);

        return this;
    }

    public RegistrationEntityBuilder WithEmailAddress(string value)
    {
        _emailAddress = new(value);

        return this;
    }

    public Registration Build() =>
        Registration.Of
        (
            _id,
            _eventId,
            _name,
            _phoneNumber,
            _emailAddress
        );
}