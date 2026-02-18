namespace Events.Domain.Events;

public sealed class LocationTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Location(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Location(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        string tooLongLocationValue = new('*', Location.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => new Location(tooLongLocationValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        const string locationValue = "Test";

        Location location = new(locationValue);

        Assert.Equal(new(locationValue), location);
    }
}