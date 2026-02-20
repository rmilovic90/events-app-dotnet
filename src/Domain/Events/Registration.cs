namespace Events.Domain.Events;

public sealed class Registration
{
    public static Registration New
    (
        Id eventId,
        RegistrationName name,
        RegistrationPhoneNumber phoneNumber,
        RegistrationEmailAddress emailAddress
    ) => Of
    (
        new Id(),
        eventId,
        name,
        phoneNumber,
        emailAddress
    );

    public static Registration Of
    (
        Id id,
        Id eventId,
        RegistrationName name,
        RegistrationPhoneNumber phoneNumber,
        RegistrationEmailAddress emailAddress
    ) => new
    (
        id,
        eventId,
        name,
        phoneNumber,
        emailAddress
    );

    private Registration
    (
        Id id,
        Id eventId,
        RegistrationName name,
        RegistrationPhoneNumber phoneNumber,
        RegistrationEmailAddress emailAddress
    )
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(eventId);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(phoneNumber);
        ArgumentNullException.ThrowIfNull(emailAddress);

        Id = id;
        EventId = eventId;
        Name = name;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    public Id Id { get; }
    public Id EventId { get; }
    public RegistrationName Name { get; }
    public RegistrationPhoneNumber PhoneNumber { get; }
    public RegistrationEmailAddress EmailAddress { get; }

    public override string ToString() =>
        $"{nameof(Registration)} {{ {nameof(Id)} = {Id}, {nameof(EventId)} = {EventId}, {nameof(Name)} = {Name}, {nameof(PhoneNumber)} = {PhoneNumber}, {nameof(EmailAddress)} = {EmailAddress} }}";
}