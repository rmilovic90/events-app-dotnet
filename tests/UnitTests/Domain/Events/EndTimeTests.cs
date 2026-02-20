using Microsoft.Extensions.Time.Testing;

namespace Events.Domain.Events;

public sealed class EndTimeTests
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset UtcTomorrow = UtcNow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    [Fact]
    public void Create_Fails_WhenBeforeStartTime()
    {
        DateTimeOffset utcYesteday = UtcNow.Date.AddDays(-1);
        StartTime startTime = StartTime.New
        (
            new DateTimeOffset
            (
                UtcTomorrow.Year,
                UtcTomorrow.Month,
                UtcTomorrow.Day,
                UtcTomorrow.Hour,
                UtcTomorrow.Minute,
                UtcTomorrow.Second,
                CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)
            ),
            new FakeTimeProvider(UtcNow)
        );

        Assert.Throws<ArgumentException>(() => EndTime.Of(utcYesteday, startTime));
    }

    [Fact]
    public void Create_Fails_WhenValueIsSameAsStartTime()
    {
        StartTime startTime = StartTime.New(UtcTomorrow, new FakeTimeProvider(UtcNow));

        Assert.Throws<ArgumentException>(() => EndTime.Of(UtcTomorrow, startTime));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsAfterStartTime()
    {
        DateTimeOffset utcDayAfterTomorrow = UtcTomorrow.AddDays(1);
        StartTime startTime = StartTime.New
        (
            new DateTimeOffset
            (
                UtcTomorrow.Year,
                UtcTomorrow.Month,
                UtcTomorrow.Day,
                UtcTomorrow.Hour,
                UtcTomorrow.Minute,
                UtcTomorrow.Second,
                CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)
            ),
            new FakeTimeProvider(UtcNow)
        );

        EndTime endTime = EndTime.Of(utcDayAfterTomorrow, startTime);

        Assert.Equal(utcDayAfterTomorrow, endTime.Value);
    }
}