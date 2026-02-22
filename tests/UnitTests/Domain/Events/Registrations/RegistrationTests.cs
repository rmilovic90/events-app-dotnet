using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;

namespace Events.Domain.Events.Registrations;

public sealed class RegistrationTests
{
    [Fact]
    public void CreateOfNewRegistration_Fails_WhenEventIdIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Registration.New
            (
                null!,
                ARegistrationName,
                ARegistrationPhoneNumber,
                ARegistrationEmailAddress
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
                ARegistrationEventId,
                null!,
                ARegistrationPhoneNumber,
                ARegistrationEmailAddress
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
                ARegistrationEventId,
                ARegistrationName,
                null!,
                ARegistrationEmailAddress
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
                ARegistrationEventId,
                ARegistrationName,
                ARegistrationPhoneNumber,
                null!
            )
        );
    }

    [Fact]
    public void CreateOfNewRegistration_Succeeds_WhenAllParametersAreProvided()
    {
        Registration registration = Registration.New
        (
            ARegistrationEventId,
            ARegistrationName,
            ARegistrationPhoneNumber,
            ARegistrationEmailAddress
        );

        Assert.Multiple
        (
            () => Assert.NotNull(registration.Id),
            () => Assert.Equal(ARegistrationEventId, registration.EventId),
            () => Assert.Equal(ARegistrationName, registration.Name),
            () => Assert.Equal(ARegistrationPhoneNumber, registration.PhoneNumber),
            () => Assert.Equal(ARegistrationEmailAddress, registration.EmailAddress)
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
                ARegistrationEventId,
                ARegistrationName,
                ARegistrationPhoneNumber,
                ARegistrationEmailAddress
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
                ARegistrationId,
                null!,
                ARegistrationName,
                ARegistrationPhoneNumber,
                ARegistrationEmailAddress
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
                ARegistrationId,
                ARegistrationEventId,
                null!,
                ARegistrationPhoneNumber,
                ARegistrationEmailAddress
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
                ARegistrationId,
                ARegistrationEventId,
                ARegistrationName,
                null!,
                ARegistrationEmailAddress
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
                ARegistrationId,
                ARegistrationEventId,
                ARegistrationName,
                ARegistrationPhoneNumber,
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
            ARegistrationEventId,
            ARegistrationName,
            ARegistrationPhoneNumber,
            ARegistrationEmailAddress
        );

        Assert.Multiple
        (
            () => Assert.Equal(id, registration.Id),
            () => Assert.Equal(ARegistrationEventId, registration.EventId),
            () => Assert.Equal(ARegistrationName, registration.Name),
            () => Assert.Equal(ARegistrationPhoneNumber, registration.PhoneNumber),
            () => Assert.Equal(ARegistrationEmailAddress, registration.EmailAddress)
        );
    }
}