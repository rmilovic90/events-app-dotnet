using Microsoft.Extensions.Time.Testing;

namespace Events.Domain.Events;

public sealed class EventTests
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset UtcTomorrow = UtcNow.AddDays(1);
    private static readonly DateTimeOffset UtcDayAfterTomorrow = UtcTomorrow.AddDays(1);

    private static readonly Name Name = new("Test");
    private static readonly Description Description = new("Test event.");
    private static readonly Location Location = new("Novi Sad, Serbia");
    private static readonly StartTime StartTime = StartTime.New(UtcTomorrow, new FakeTimeProvider(UtcNow));
    private static readonly EndTime EndTime = EndTime.Of(UtcDayAfterTomorrow, StartTime);

    [Fact]
    public void CreateOfNewEvent_Fails_WhenNameIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.New
            (
                null!,
                Description,
                Location,
                StartTime,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfNewEvent_Fails_WhenDescriptionIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.New
            (
                Name,
                null!,
                Location,
                StartTime,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfNewEvent_Fails_WhenLocationIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.New
            (
                Name,
                Description,
                null!,
                StartTime,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfNewEvent_Fails_WhenStartTimeIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.New
            (
                Name,
                Description,
                Location,
                null!,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfNewEvent_Fails_WhenEndTimeIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.New
            (
                Name,
                Description,
                Location,
                StartTime,
                null!
            )
        );
    }

    [Fact]
    public void CreateOfNewEvent_Succeeds_WhenAllParametersAreProvided()
    {
        Event @event = Event.New
        (
            Name,
            Description,
            Location,
            StartTime,
            EndTime
        );

        Assert.Multiple
        (
            () => Assert.NotNull(@event.Id),
            () => Assert.Equal(Name, @event.Name),
            () => Assert.Equal(Description, @event.Description),
            () => Assert.Equal(Location, @event.Location),
            () => Assert.Equal(StartTime, @event.StartTime),
            () => Assert.Equal(EndTime, @event.EndTime)
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Fails_WhenIdIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.Of
            (
                null!,
                Name,
                Description,
                Location,
                StartTime,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Fails_WhenNameIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.Of
            (
                new Id(),
                null!,
                Description,
                Location,
                StartTime,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Fails_WhenDescriptionIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.Of
            (
                new Id(),
                Name,
                null!,
                Location,
                StartTime,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Fails_WhenLocationIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.Of
            (
                new Id(),
                Name,
                Description,
                null!,
                StartTime,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Fails_WhenStartTimeIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.Of
            (
                new Id(),
                Name,
                Description,
                Location,
                null!,
                EndTime
            )
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Fails_WhenEndTimeIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.Of
            (
                new Id(),
                Name,
                Description,
                Location,
                StartTime,
                null!
            )
        );
    }

    [Fact]
    public void CreateOfExistingEvent_Succeeds_WhenAllParametersAreProvided()
    {
        Id id = new();
        Event @event = Event.Of
        (
            id,
            Name,
            Description,
            Location,
            StartTime,
            EndTime
        );

        Assert.Multiple
        (
            () => Assert.Equal(id, @event.Id),
            () => Assert.Equal(Name, @event.Name),
            () => Assert.Equal(Description, @event.Description),
            () => Assert.Equal(Location, @event.Location),
            () => Assert.Equal(StartTime, @event.StartTime),
            () => Assert.Equal(EndTime, @event.EndTime)
        );
    }

    [Fact]
    public void DoesNotAllowAddingOfPendingRegistration_WhenRegistrationIsNull()
    {
        Event @event = Event.New
        (
            Name,
            Description,
            Location,
            StartTime,
            EndTime
        );

        Assert.Throws<ArgumentNullException>(() => @event.Add(null!));
    }

    [Fact]
    public void AllowsAddingOfPendingRegistration_WhenRegistrationIsPresent()
    {
        Id registrationId = new();
        Id registrationEventId = new();
        RegistrationName registrationName = new("Jane Doe");
        RegistrationPhoneNumber registrationPhoneNumber = new("+38155555555");
        RegistrationEmailAddress registrationEmailAddress = new("jane.doe@email.com");

        Event @event = Event.New
        (
            Name,
            Description,
            Location,
            StartTime,
            EndTime
        );

        @event.Add
        (
            Registration.Of
            (
                registrationId,
                registrationEventId,
                registrationName,
                registrationPhoneNumber,
                registrationEmailAddress
            )
        );

        Registration pendingRegistration = Assert.Single(@event.PendingRegistrations);
        Assert.Multiple
        (
            () => Assert.Equal(registrationId, pendingRegistration.Id),
            () => Assert.Equal(registrationEventId, pendingRegistration.EventId),
            () => Assert.Equal(registrationName, pendingRegistration.Name),
            () => Assert.Equal(registrationPhoneNumber, pendingRegistration.PhoneNumber),
            () => Assert.Equal(registrationEmailAddress, pendingRegistration.EmailAddress)
        );
    }
}