namespace Events.Domain.Events.Registrations;

public sealed class Registration
{
    public static Registration New
    (
        Id eventId,
        Name name,
        PhoneNumber phoneNumber,
        EmailAddress emailAddress
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
        Name name,
        PhoneNumber phoneNumber,
        EmailAddress emailAddress
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
        Name name,
        PhoneNumber phoneNumber,
        EmailAddress emailAddress
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
    public Name Name { get; }
    public PhoneNumber PhoneNumber { get; }
    public EmailAddress EmailAddress { get; }

    public override string ToString() =>
        $"{nameof(Registration)} {{ {nameof(Id)} = {Id}, {nameof(EventId)} = {EventId}, {nameof(Name)} = {Name}, {nameof(PhoneNumber)} = {PhoneNumber}, {nameof(EmailAddress)} = {EmailAddress} }}";
}