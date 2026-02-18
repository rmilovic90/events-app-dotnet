using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Events.WebApi.Common.Validation;

using EventDescription = Events.Domain.Events.Description;
using EventLocation = Events.Domain.Events.Location;
using EventName = Events.Domain.Events.Name;

namespace Events.WebApi.Events;

public sealed class Event
{
    [Required]
    [MaxLength(EventName.MaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(EventDescription.MaxLength)]
    public string Description { get; set; } = null!;

    [Required]
    [MaxLength(EventLocation.MaxLength)]
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