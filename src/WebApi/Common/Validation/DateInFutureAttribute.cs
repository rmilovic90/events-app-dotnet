using System.ComponentModel.DataAnnotations;

namespace Events.WebApi.Common.Validation;

internal sealed class DateInFutureAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "The {0} date must be in the future.";

    public DateInFutureAttribute() : base(DefaultErrorMessage) { }

    public DateInFutureAttribute(string errorMessage) : base(errorMessage) { }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        if (value is not DateTime dateTimeValue)
            throw new InvalidOperationException($"{nameof(DateInFutureAttribute)} can only be applied to values of type {nameof(DateTime)}.");

        DateTime now = dateTimeValue.Kind == DateTimeKind.Utc
            ? DateTime.UtcNow
            : DateTime.Now;

        return dateTimeValue <= now
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName! })
            : ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name) =>
        string.Format(ErrorMessageString, name);
}