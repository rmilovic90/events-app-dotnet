using static Events.Domain.Events.EventEntityBuilder;

namespace Events.Domain.Events;

public sealed class NameTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Name(null!));
    }

    [Theory]
    [InlineData(AnEmptyEventNameValue)]
    [InlineData(EventNameValueWithWhitespacesOnly)]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Name(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        Assert.Throws<ArgumentException>(() => new Name(TooLongEventNameValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        Name name = new(AnEventNameValue);

        Assert.Equal(new(AnEventNameValue), name);
    }
}