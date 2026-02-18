using Microsoft.Extensions.Time.Testing;

namespace Events.Domain.Events;

public sealed class StartTimeTests
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    [Fact]
    public void Create_Fails_WhenValueIsInThePast()
    {
        DateTimeOffset utcYesteday = UtcNow.AddDays(-1);

        Assert.Throws<ArgumentException>
        (
            () => StartTime.Of
            (
                new DateTimeOffset
                (
                    utcYesteday.Year,
                    utcYesteday.Month,
                    utcYesteday.Day,
                    utcYesteday.Hour,
                    utcYesteday.Minute,
                    utcYesteday.Second,
                    CentralEuropeanTimeZone.GetUtcOffset(utcYesteday)
                ),
                new FakeTimeProvider(UtcNow)
            )
        );
    }

    [Fact]
    public void Create_Fails_WhenValueIsSameAsCurrentTime()
    {
        Assert.Throws<ArgumentException>(() => StartTime.Of(UtcNow, new FakeTimeProvider(UtcNow)));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsInTheFuture()
    {
        DateTimeOffset utcTomorrow = UtcNow.AddDays(1);
        DateTimeOffset utcTomorrowCentralEuropeanTime = new
        (
            utcTomorrow.Year,
            utcTomorrow.Month,
            utcTomorrow.Day,
            utcTomorrow.Hour,
            utcTomorrow.Minute,
            utcTomorrow.Second,
            CentralEuropeanTimeZone.GetUtcOffset(utcTomorrow)
        );

        StartTime startTime = StartTime.Of(utcTomorrowCentralEuropeanTime, new FakeTimeProvider(UtcNow));

        Assert.Equal(utcTomorrowCentralEuropeanTime, startTime.Value);
    }
}