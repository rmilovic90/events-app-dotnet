using Events.Domain.Events.Registrations;

using static Events.Domain.Events.EventEntityBuilder;
using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;

namespace Events.Domain.Events;

public sealed class EventTests
{
    [Fact]
    public void CreateOfNewEvent_Fails_WhenNameIsNull()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => Event.New
            (
                null!,
                AnEventDescription,
                AnEventLocation,
                TomorrowStartTime,
                FollowingDayEndTime
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
                AnEventName,
                null!,
                AnEventLocation,
                TomorrowStartTime,
                FollowingDayEndTime
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
                AnEventName,
                AnEventDescription,
                null!,
                TomorrowStartTime,
                FollowingDayEndTime
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
                AnEventName,
                AnEventDescription,
                AnEventLocation,
                null!,
                FollowingDayEndTime
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
                AnEventName,
                AnEventDescription,
                AnEventLocation,
                TomorrowStartTime,
                null!
            )
        );
    }

    [Fact]
    public void CreateOfNewEvent_Succeeds_WhenAllParametersAreProvided()
    {
        Event @event = Event.New
        (
            AnEventName,
            AnEventDescription,
            AnEventLocation,
            TomorrowStartTime,
            FollowingDayEndTime
        );

        Assert.Multiple
        (
            () => Assert.NotNull(@event.Id),
            () => Assert.Equal(AnEventName, @event.Name),
            () => Assert.Equal(AnEventDescription, @event.Description),
            () => Assert.Equal(AnEventLocation, @event.Location),
            () => Assert.Equal(TomorrowStartTime, @event.StartTime),
            () => Assert.Equal(FollowingDayEndTime, @event.EndTime)
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
                AnEventName,
                AnEventDescription,
                AnEventLocation,
                TomorrowStartTime,
                FollowingDayEndTime
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
                AnEventId,
                null!,
                AnEventDescription,
                AnEventLocation,
                TomorrowStartTime,
                FollowingDayEndTime
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
                AnEventId,
                AnEventName,
                null!,
                AnEventLocation,
                TomorrowStartTime,
                FollowingDayEndTime
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
                AnEventId,
                AnEventName,
                AnEventDescription,
                null!,
                TomorrowStartTime,
                FollowingDayEndTime
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
                AnEventId,
                AnEventName,
                AnEventDescription,
                AnEventLocation,
                null!,
                FollowingDayEndTime
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
                AnEventId,
                AnEventName,
                AnEventDescription,
                AnEventLocation,
                TomorrowStartTime,
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
            AnEventName,
            AnEventDescription,
            AnEventLocation,
            TomorrowStartTime,
            FollowingDayEndTime
        );

        Assert.Multiple
        (
            () => Assert.Equal(id, @event.Id),
            () => Assert.Equal(AnEventName, @event.Name),
            () => Assert.Equal(AnEventDescription, @event.Description),
            () => Assert.Equal(AnEventLocation, @event.Location),
            () => Assert.Equal(TomorrowStartTime, @event.StartTime),
            () => Assert.Equal(FollowingDayEndTime, @event.EndTime)
        );
    }

    [Fact]
    public void DoesNotAllowAddingOfPendingRegistration_WhenRegistrationIsNull()
    {
        Event @event = ANewEventEntity.Build();

        Assert.Throws<ArgumentNullException>(() => @event.Add(null!));
    }

    [Fact]
    public void AllowsAddingOfPendingRegistration_WhenRegistrationIsPresent()
    {
        Event @event = ANewEventEntity.Build();
        Registration registration = ANewRegistrationEntity.Build();

        @event.Add(registration);

        Assert.Equivalent
        (
            new[] { registration },
            @event.PendingRegistrations
        );
    }
}