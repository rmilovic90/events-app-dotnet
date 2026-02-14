using System.ComponentModel.DataAnnotations;

namespace Events.WebApi.Common.Validation;

internal sealed class DateInFutureAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "The {0} date must be in the future.";

    public DateInFutureAttribute() : base(DefaultErrorMessage) { }

    public DateInFutureAttribute(string errorMessage) : base(errorMessage) { }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        value switch
        {
            null => ValidationResult.Success,
            DateTime dateTimeValue => dateTimeValue.ToUniversalTime() <= DateTime.UtcNow
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName! })
                : ValidationResult.Success,
            DateTimeOffset dateTimeOffsetValue => dateTimeOffsetValue.ToUniversalTime() <= DateTimeOffset.UtcNow
                ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName! })
                : ValidationResult.Success,
            _ => throw new InvalidOperationException($"{nameof(DateInFutureAttribute)} can only be applied to values of type {nameof(DateTime)} or {nameof(DateTimeOffset)}.")
        };

    public override string FormatErrorMessage(string name) =>
        string.Format(ErrorMessageString, name);
}