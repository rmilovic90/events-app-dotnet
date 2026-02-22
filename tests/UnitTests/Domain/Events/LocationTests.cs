using static Events.Domain.Events.EventEntityBuilder;

namespace Events.Domain.Events;

public sealed class LocationTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Location(null!));
    }

    [Theory]
    [InlineData(AnEmptyEventLocationValue)]
    [InlineData(EventLocationValueWithWhitespacesOnly)]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Location(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        string tooLongLocationValue = new('*', Location.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => new Location(TooLongEventLocationValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        Location location = new(AnEventLocationValue);

        Assert.Equal(new(AnEventLocationValue), location);
    }
}