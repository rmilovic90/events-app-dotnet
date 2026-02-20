namespace Events.Domain.Events;

public sealed class RegistrationTests
{
    private static readonly Id EventId = new();
    private static readonly RegistrationName Name = new("Jane Doe");
    private static readonly RegistrationPhoneNumber PhoneNumber = new("+38155555555");
    private static readonly RegistrationEmailAddress EmailAddress = new("jane.doe@email.com");

    [Fact]
    public void CreateOfNewRegistration_Fails_WhenEventIdIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.New
            (
                null!,
                Name,
                PhoneNumber,
                EmailAddress
            )
        );
    }

    [Fact]
    public void CreateOfNewRegistration_Fails_WhenNameIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.New
            (
                EventId,
                null!,
                PhoneNumber,
                EmailAddress
            )
        );
    }

    [Fact]
    public void CreateOfNewRegistration_Fails_WhenPhoneNumberIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.New
            (
                EventId,
                Name,
                null!,
                EmailAddress
            )
        );
    }

    [Fact]
    public void CreateOfNewRegistration_Fails_WhenEmailAddressIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.New
            (
                EventId,
                Name,
                PhoneNumber,
                null!
            )
        );
    }

    [Fact]
    public void CreateOfNewRegistration_Succeeds_WhenAllParametersAreProvided()
    {
        Registration registration = Registration.New
        (
            EventId,
            Name,
            PhoneNumber,
            EmailAddress
        );

        Assert.Multiple
        (
            () => Assert.NotNull(registration.Id),
            () => Assert.Equal(EventId, registration.EventId),
            () => Assert.Equal(Name, registration.Name),
            () => Assert.Equal(PhoneNumber, registration.PhoneNumber),
            () => Assert.Equal(EmailAddress, registration.EmailAddress)
        );
    }

    [Fact]
    public void CreateOfExistingRegistration_Fails_WhenIdIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.Of
            (
                null!,
                EventId,
                Name,
                PhoneNumber,
                EmailAddress
            )
        );
    }

    [Fact]
    public void CreateOfExistingRegistration_Fails_WhenEventIdIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.Of
            (
                new Id(),
                null!,
                Name,
                PhoneNumber,
                EmailAddress
            )
        );
    }

    [Fact]
    public void CreateOfExistingRegistration_Fails_WhenNameIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.Of
            (
                new Id(),
                EventId,
                null!,
                PhoneNumber,
                EmailAddress
            )
        );
    }

    [Fact]
    public void CreateOfExistingRegistration_Fails_WhenPhoneNumberIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.Of
            (
                new Id(),
                EventId,
                Name,
                null!,
                EmailAddress
            )
        );
    }

    [Fact]
    public void CreateOfExistingRegistration_Fails_WhenEmailAddressIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.Of
            (
                new Id(),
                EventId,
                Name,
                PhoneNumber,
                null!
            )
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Succeeds_WhenAllParametersAreProvided()
    {
        Id id = new();
        Registration registration = Registration.Of
        (
            id,
            EventId,
            Name,
            PhoneNumber,
            EmailAddress
        );

        Assert.Multiple
        (
            () => Assert.Equal(id, registration.Id),
            () => Assert.Equal(EventId, registration.EventId),
            () => Assert.Equal(Name, registration.Name),
            () => Assert.Equal(PhoneNumber, registration.PhoneNumber),
            () => Assert.Equal(EmailAddress, registration.EmailAddress)
        );
    }
}