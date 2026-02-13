using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Events.WebApi.Common.Validation;

internal sealed class DateAfterAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "The {0} date must be after {1} date.";

    private readonly string _comparisonDatePropertyName;

    public DateAfterAttribute(string comparisonDatePropertyName) : this(comparisonDatePropertyName, DefaultErrorMessage) { }

    public DateAfterAttribute(string comparisonDatePropertyName, string errorMessage) : base(errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comparisonDatePropertyName);

        _comparisonDatePropertyName = comparisonDatePropertyName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        if (value is not DateTime dateTimeValue)
            throw new InvalidOperationException($"{nameof(DateAfterAttribute)} can only be applied to properties of type {nameof(DateTime)}.");

        PropertyInfo comparisonDateProperty = validationContext.ObjectInstance.GetType().GetProperty(_comparisonDatePropertyName)
            ?? throw new InvalidOperationException($"{nameof(DateAfterAttribute)} comparison property {_comparisonDatePropertyName} does not exist.");

        object comparisonDatePropertyValue = comparisonDateProperty.GetValue(validationContext.ObjectInstance)
            ?? throw new InvalidOperationException($"{nameof(DateAfterAttribute)} comparison property {_comparisonDatePropertyName} must have a value.");

        if (comparisonDatePropertyValue is not DateTime comparisonDatePropertyDateTimeValue)
            throw new InvalidOperationException($"{nameof(DateAfterAttribute)} comparison property {_comparisonDatePropertyName} must be of type {nameof(DateTime)}.");

        DateTime dateTimeValueInUtc = dateTimeValue.Kind == DateTimeKind.Utc
            ? dateTimeValue
            : dateTimeValue.ToUniversalTime();

        DateTime comparisonDatePropertyDateTimeValueInUtc = comparisonDatePropertyDateTimeValue.Kind == DateTimeKind.Utc
            ? comparisonDatePropertyDateTimeValue
            : comparisonDatePropertyDateTimeValue.ToUniversalTime();

        return dateTimeValueInUtc <= comparisonDatePropertyDateTimeValueInUtc
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName! })
            : ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name) =>
        string.Format(ErrorMessageString, name, _comparisonDatePropertyName);
}