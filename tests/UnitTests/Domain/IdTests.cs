namespace Events.Domain;

public sealed class IdTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Id(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Id(value!));
    }

    [Fact]
    public void Create_SucceedsWithGivenValue_WhenValueIsProvided()
    {
        const string idValue = "ID_12345";

        Id id = new(idValue);

        Assert.Equal(new(idValue), id);
    }

    [Fact]
    public void Create_SucceedsWithGeneratedValue_WhenValueIsNotProvided()
    {
        Id id = new();

        Assert.NotEmpty(id.ToString());
    }
}