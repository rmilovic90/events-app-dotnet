using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;

namespace Events.Domain.Events.Registrations;

public sealed class PhoneNumberTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new PhoneNumber(null!));
    }

    [Theory]
    [InlineData(AnEmptyRegistrationPhoneNumberValue)]
    [InlineData(RegistrationPhoneNumberValueWithWhitespacesOnly)]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new PhoneNumber(value!));
    }

    [Theory]
    [InlineData("38155555555", Label = "Does not start with '+' sign.")]
    [InlineData("+3815555555555555", Label = "Has more then 15 digits.")]
    [InlineData("+1", Label = "Has only single digit country code.")]
    [InlineData("+0381555555555", Label = "Has leading zero as first digit after '+' sign.")]
    [InlineData("+1 (55)a555-555.", Label = "Contains characters other then digits and '+' sign.")]
    public void Create_Fails_WhenValueHasWrongFormat(string value)
    {
        Assert.Throws<ArgumentException>(() => new PhoneNumber(value));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        PhoneNumber phoneNumber = new(ARegistrationPhoneNumberValue);

        Assert.Equal(new(ARegistrationPhoneNumberValue), phoneNumber);
    }
}