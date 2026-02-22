namespace Events.Domain.Events;

public sealed class EndTimeTests
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset UtcTomorrow = UtcNow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    [Fact]
    public void CreationOfNewEndTime_Fails_WhenBeforeStartTime()
    {
        DateTimeOffset utcYesteday = UtcNow.Date.AddDays(-1);
        StartTime startTime = StartTime.Of
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
            )
        );

        Assert.Throws<ArgumentException>(() => EndTime.New(utcYesteday, startTime));
    }

    [Fact]
    public void CreationOfNewEndTime_Fails_WhenValueIsSameAsStartTime()
    {
        StartTime startTime = StartTime.Of(UtcTomorrow);

        Assert.Throws<ArgumentException>(() => EndTime.New(UtcTomorrow, startTime));
    }

    [Fact]
    public void CreationOfNewEndTime_Succeeds_WhenValueIsAfterStartTime()
    {
        DateTimeOffset utcDayAfterTomorrow = UtcTomorrow.AddDays(1);
        StartTime startTime = StartTime.Of
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
            )
        );

        EndTime endTime = EndTime.New(utcDayAfterTomorrow, startTime);

        Assert.Equal(utcDayAfterTomorrow, endTime.Value);
    }
}