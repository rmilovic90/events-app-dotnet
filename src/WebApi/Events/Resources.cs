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
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    [Required]
    [DateAfter(nameof(StartTime))]
    public DateTime EndTime { get; set; } = DateTime.UtcNow;
}