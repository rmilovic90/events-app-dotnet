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

        DateTime utcDateTimeValue = GetUtcDateTimeFromValue(value);
        DateTime utcComparisonDateDateTimeValue = GetUtcDateTimeFromComparisonDateProperty(validationContext);

        return utcDateTimeValue <= utcComparisonDateDateTimeValue
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName! })
            : ValidationResult.Success;
    }

    private static DateTime GetUtcDateTimeFromValue(object value)
    {
        DateTime? dateTimeValue = value is DateTime dateTime ? dateTime : null;
        DateTimeOffset? dateTimeOffsetValue = value is DateTimeOffset dateTimeOffset ? dateTimeOffset : null;

        return (dateTimeValue?.ToUniversalTime() ?? dateTimeOffsetValue?.ToUniversalTime().DateTime)
            ?? throw new InvalidOperationException($"{nameof(DateAfterAttribute)} can only be applied to values of type {nameof(DateTime)} && {nameof(DateTimeOffset)}.");
    }

    private DateTime GetUtcDateTimeFromComparisonDateProperty(ValidationContext validationContext)
    {
        PropertyInfo comparisonDateProperty = validationContext.ObjectInstance.GetType().GetProperty(_comparisonDatePropertyName)
            ?? throw new InvalidOperationException($"{nameof(DateAfterAttribute)} comparison property {_comparisonDatePropertyName} does not exist.");

        object comparisonDatePropertyValue = comparisonDateProperty.GetValue(validationContext.ObjectInstance)
            ?? throw new InvalidOperationException($"{nameof(DateAfterAttribute)} comparison property {_comparisonDatePropertyName} must have a value.");

        DateTime? comparisonDateTimeValue = comparisonDatePropertyValue is DateTime comparisonDateTime ? comparisonDateTime : null;
        DateTimeOffset? comparisonDateTimeOffsetValue = comparisonDatePropertyValue is DateTimeOffset comparisonDateTimeOffset ? comparisonDateTimeOffset : null;

        return (comparisonDateTimeValue?.ToUniversalTime() ?? comparisonDateTimeOffsetValue?.ToUniversalTime().DateTime)
            ?? throw new InvalidOperationException($"{nameof(DateAfterAttribute)} comparison property {_comparisonDatePropertyName} must be of type {nameof(DateTime)} or {nameof(DateTimeOffset)}.");
    }

    public override string FormatErrorMessage(string name) =>
        string.Format(ErrorMessageString, name, _comparisonDatePropertyName);
}