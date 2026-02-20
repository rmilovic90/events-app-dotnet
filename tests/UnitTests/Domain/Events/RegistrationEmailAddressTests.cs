namespace Events.Domain.Events;

public sealed class RegistrationEmailAddressTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RegistrationEmailAddress(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new RegistrationEmailAddress(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        string tooLongEmailAddressValue = new('*', RegistrationEmailAddress.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => new RegistrationEmailAddress(tooLongEmailAddressValue));
    }

    [Theory]
    [InlineData("jane.doe.email.com", Label = "Does not have '@' character.")]
    [InlineData("@jane.doe.email.com", Label = "Has '@' character as first character.")]
    [InlineData("jane.doe.email.com@", Label = "Has '@' character as last character.")]
    [InlineData("jane@doe@email.com", Label = "Has multiple '@' characters that are not at the edges.")]
    [InlineData(" jane.doe @ email.com ", Label = "Contains whitespace characters.")]
    [InlineData("jane.doe@\nemail.com", Label = "Contains Unix line break character.")]
    [InlineData("jane.doe@\r\nemail.com", Label = "Contains Windows line break character.")]
    public void Create_Fails_WhenValueHasWrongFormat(string value)
    {
        Assert.Throws<ArgumentException>(() => new RegistrationEmailAddress(value));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        const string emailAddressValue = "jane.doe@email.com";

        RegistrationEmailAddress emailAddress = new(emailAddressValue);

        Assert.Equal(new(emailAddressValue), emailAddress);
    }
}