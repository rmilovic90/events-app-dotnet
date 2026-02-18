namespace Events.Domain.Events;

public sealed class NameTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Name(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Name(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        string tooLongNameValue = new('*', Name.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => new Name(tooLongNameValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        const string nameValue = "Test";

        Name name = new(nameValue);

        Assert.Equal(new(nameValue), name);
    }
}