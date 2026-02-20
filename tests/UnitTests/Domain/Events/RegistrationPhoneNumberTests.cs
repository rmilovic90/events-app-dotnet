namespace Events.Domain.Events;

public sealed class RegistrationPhoneNumberTests
{
    [Fact]
    public void Create_Fails_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new RegistrationPhoneNumber(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Fails_WhenValueIsBlank(string? value)
    {
        Assert.Throws<ArgumentException>(() => new RegistrationPhoneNumber(value!));
    }

    [Theory]
    [InlineData("38155555555", Label = "Does not start with '+' sign.")]
    [InlineData("+3815555555555555", Label = "Has more then 15 digits.")]
    [InlineData("+1", Label = "Has only single digit country code.")]
    [InlineData("+0381555555555", Label = "Has leading zero as first digit after '+' sign.")]
    [InlineData("+1 (55)a555-555.", Label = "Contains characters other then digits and '+' sign.")]
    public void Create_Fails_WhenValueHasWrongFormat(string value)
    {
        Assert.Throws<ArgumentException>(() => new RegistrationPhoneNumber(value));
    }

    [Fact]
    public void Create_Succeeds_WhenValueIsProvided()
    {
        const string phoneNumberValue = "+38155555555";

        RegistrationPhoneNumber phoneNumber = new(phoneNumberValue);

        Assert.Equal(new(phoneNumberValue), phoneNumber);
    }
}