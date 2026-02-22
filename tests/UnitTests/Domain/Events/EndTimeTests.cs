using static Events.Domain.Events.EventEntityBuilder;

namespace Events.Domain.Events;

public sealed class EndTimeTests
{
    [Fact]
    public void CreationOfNewEndTime_Fails_WhenBeforeStartTime()
    {
        Assert.Throws<ArgumentException>(() => EndTime.New(TodayEventTimeValue, TomorrowStartTime));
    }

    [Fact]
    public void CreationOfNewEndTime_Fails_WhenValueIsSameAsStartTime()
    {
        Assert.Throws<ArgumentException>(() => EndTime.New(TomorrowEventTimeValue, TomorrowStartTime));
    }

    [Fact]
    public void CreationOfNewEndTime_Succeeds_WhenValueIsAfterStartTime()
    {
        EndTime endTime = EndTime.New(FollowingDayEventTimeValue, TomorrowStartTime);

        Assert.Equal(EndTime.Of(FollowingDayEventTimeValue), endTime);
    }
}