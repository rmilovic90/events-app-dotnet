namespace Events.Domain.Events.Registrations;

public sealed class RegistrationEntityBuilder
{
    public const string AnEmptyRegistrationNameValue = "";
    public const string RegistrationNameValueWithWhitespacesOnly = "  ";
    public static readonly string TooLongRegistrationNameValue = new('*', RegistrationName.MaxLength + 1);
    private const string ARegistrationNameValue = "Jane Doe";
    public const string AnEmptyRegistrationPhoneNumberValue = "";
    public const string RegistrationPhoneNumberValueWithWhitespacesOnly = "  ";
    public const string ARegistrationPhoneNumberValue = "+38155555555";
    public const string AnEmptyRegistrationEmailAddressValue = "";
    public const string RegistrationEmailAddressValueWithWhitespacesOnly = "  ";
    public static readonly string TooLongRegistrationEmailAddressValue = $"{new('*', RegistrationEmailAddress.MaxLength + 1)}@email.com";
    public const string ARegistrationEmailAddressValue = "jane.doe@email.com";

    public static readonly Id ARegistrationId = new();
    public static readonly Id ARegistrationEventId = new();
    public static readonly RegistrationName ARegistrationName = new(ARegistrationNameValue);
    public static readonly RegistrationPhoneNumber ARegistrationPhoneNumber = new(ARegistrationPhoneNumberValue);
    public static readonly RegistrationEmailAddress ARegistrationEmailAddress = new(ARegistrationEmailAddressValue);

    public static RegistrationEntityBuilder ANewRegistrationEntity => new(new Id());

    private readonly Id _id = ARegistrationId;
    private Id _eventId = ARegistrationEventId;
    private RegistrationName _name = ARegistrationName;
    private RegistrationPhoneNumber _phoneNumber = ARegistrationPhoneNumber;
    private RegistrationEmailAddress _emailAddress = ARegistrationEmailAddress;

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