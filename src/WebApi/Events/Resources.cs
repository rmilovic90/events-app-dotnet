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
    [DateMinDaysInTheFuture(1)]
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    // TODO: Ensure that end time is after start time (event must last at least N period).
    [Required]
    public DateTime EndTime { get; set; } = DateTime.UtcNow;
}