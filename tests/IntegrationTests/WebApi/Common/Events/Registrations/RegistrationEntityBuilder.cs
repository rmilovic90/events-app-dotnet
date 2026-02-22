using Events.Domain;
using Events.Domain.Events;

namespace Events.WebApi.Common.Events.Registrations;

public sealed class RegistrationEntityBuilder
{
    private const string DefaultName = "Jane Doe";
    private const string DefaultPhoneNumber = "+38155555555";
    private const string DefaultEmailAddress = "jane.doe@email.com";

    public static RegistrationEntityBuilder ANewRegistrationEntity => new();

    private readonly Id _id = new();
    private Id _eventId = new();
    private RegistrationName _name = new(DefaultName);
    private RegistrationPhoneNumber _phoneNumber = new(DefaultPhoneNumber);
    private RegistrationEmailAddress _emailAddress = new(DefaultEmailAddress);

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

    private RegistrationEntityBuilder() { }

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