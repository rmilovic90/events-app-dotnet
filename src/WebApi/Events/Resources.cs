using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Events.WebApi.Common.Validation;

namespace Events.WebApi.Events;

public sealed class Event
{
    public const int MaxAllowedNameLength = 50;
    public const int MaxAllowedDescriptionLength = 200;
    public const int MaxAllowedLocationLength = 100;

    [Required]
    [MaxLength(MaxAllowedNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(MaxAllowedDescriptionLength)]
    public string Description { get; set; } = null!;

    [Required]
    [MaxLength(MaxAllowedLocationLength)]
    public string Location { get; set; } = null!;

    [Required]
    [DateInFuture]
    [Description("Local start time of the event. Must be in the future.")]
    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    [DateAfter(nameof(StartTime))]
    [Description("Local end time of the event. Must be after start time.")]
    public DateTimeOffset EndTime { get; set; } = DateTimeOffset.UtcNow;
}