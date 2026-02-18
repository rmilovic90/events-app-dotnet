namespace Events.Domain.Events;

public sealed class DescriptionTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Description(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Description(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        string tooLongDescriptionValue = new('*', Description.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => new Description(tooLongDescriptionValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        const string descriptionValue = "Test";

        Description description = new(descriptionValue);

        Assert.Equal(new(descriptionValue), description);
    }
}