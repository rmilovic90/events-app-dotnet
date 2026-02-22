using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;

namespace Events.Domain.Events.Registrations;

public sealed class EmailAddressTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new EmailAddress(null!));
    }

    [Theory]
    [InlineData(AnEmptyRegistrationEmailAddressValue)]
    [InlineData(RegistrationEmailAddressValueWithWhitespacesOnly)]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new EmailAddress(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        Assert.Throws<ArgumentException>(() => new EmailAddress(TooLongRegistrationEmailAddressValue));
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
        Assert.Throws<ArgumentException>(() => new EmailAddress(value));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        EmailAddress emailAddress = new(ARegistrationEmailAddressValue);

        Assert.Equal(new(ARegistrationEmailAddressValue), emailAddress);
    }
}