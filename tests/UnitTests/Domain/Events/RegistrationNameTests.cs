using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;

namespace Events.Domain.Events;

public sealed class RegistrationNameTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RegistrationName(null!));
    }

    [Theory]
    [InlineData(AnEmptyRegistrationNameValue)]
    [InlineData(RegistrationNameValueWithWhitespacesOnly)]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new RegistrationName(value!));
    }

    [Fact]
    public void Create_Fails_WhenValueIsTooLong()
    {
        Assert.Throws<ArgumentException>(() => new RegistrationName(TooLongRegistrationNameValue));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        RegistrationName name = new(ARegistrationEmailAddressValue);

        Assert.Equal(new(ARegistrationEmailAddressValue), name);
    }
}