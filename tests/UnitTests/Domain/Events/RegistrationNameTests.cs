namespace Events.Domain.Events;

public sealed class RegistrationNameTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RegistrationName(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new RegistrationName(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        string tooLongNameValue = new('*', RegistrationName.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => new RegistrationName(tooLongNameValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        const string nameValue = "Jane Doe";

        RegistrationName name = new(nameValue);

        Assert.Equal(new(nameValue), name);
    }
}