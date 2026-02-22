using Events.WebApi.Events;

namespace Events.WebApi.Common.Events.Registrations;

public sealed class RegistrationResourceBuilder
{
    private const string DefaultName = "Jane Doe";
    private const string DefaultPhoneNumber = "+38155555555";
    private const string DefaultEmailAddress = "jane.doe@email.com";

    public static RegistrationResourceBuilder ARegistrationResource => new();

    private readonly string? _id = null;
    private readonly string? _eventId = null;
    private string _name = DefaultName;
    private string _phoneNumber = DefaultPhoneNumber;
    private string _emailAddress = DefaultEmailAddress;

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