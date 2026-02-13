using System.ComponentModel.DataAnnotations;

namespace Events.WebApi.Common.Validation;

internal sealed class DateMinDaysInTheFutureAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "The {0} field is not at least {1} day(s) in the future.";

    private readonly uint _minDaysInTheFuture;

    public DateMinDaysInTheFutureAttribute(uint minDaysInTheFuture) : this(minDaysInTheFuture, DefaultErrorMessage) { }

    public DateMinDaysInTheFutureAttribute(uint minDaysInTheFuture, string errorMessage) : base(errorMessage)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan<uint>(1, minDaysInTheFuture);

        _minDaysInTheFuture = minDaysInTheFuture;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        if (value is not DateTime dateTimeValue)
            throw new InvalidOperationException($"{nameof(DateMinDaysInTheFutureAttribute)} can only be applied to values of type {nameof(DateTime)}.");

        DateTime minAllowedDateTimeValue = dateTimeValue.Kind == DateTimeKind.Utc
            ? DateTime.UtcNow.AddDays(_minDaysInTheFuture)
            : DateTime.Now.AddDays(_minDaysInTheFuture);

        return dateTimeValue < minAllowedDateTimeValue
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName! })
            : ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name) =>
        string.Format(ErrorMessageString, name, _minDaysInTheFuture);
}