using Microsoft.Extensions.Time.Testing;

using static Events.Domain.Events.EventEntityBuilder;

namespace Events.Domain.Events;

public sealed class StartTimeTests
{
    [Fact]
    public void CreationOfNewStartTime_Fails_WhenValueIsInThePast()
    {
        Assert.Throws<ArgumentException>
        (
            () => StartTime.New
            (
                YesterdayEventTimeValue,
                new FakeTimeProvider(DateTimeOffset.UtcNow)
            )
        );
    }

    [Fact]
    public void CreationOfNewStartTime_Fails_WhenValueIsSameAsCurrentTime()
    {
        Assert.Throws<ArgumentException>(() => StartTime.New(TodayEventTimeValue, new FakeTimeProvider(TodayEventTimeValue)));
    }

    [Fact]
    public void CreationOfNewStartTime_Succeeds_WhenValueIsInTheFuture()
    {
        StartTime startTime = StartTime.New(TomorrowEventTimeValue, new FakeTimeProvider(DateTimeOffset.UtcNow));

        Assert.Equal(StartTime.Of(TomorrowEventTimeValue), startTime);
    }
}