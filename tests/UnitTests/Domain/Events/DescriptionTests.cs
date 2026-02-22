using static Events.Domain.Events.EventEntityBuilder;

namespace Events.Domain.Events;

public sealed class DescriptionTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Description(null!));
    }

    [Theory]
    [InlineData(AnEmptyEventDescriptionValue)]
    [InlineData(EventDescriptionValueWithWhitespacesOnly)]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Description(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        Assert.Throws<ArgumentException>(() => new Description(TooLongEventDescriptionValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        Description description = new(AnEventDescriptionValue);

        Assert.Equal(new(AnEventDescriptionValue), description);
    }
}