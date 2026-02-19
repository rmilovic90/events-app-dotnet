using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Events.WebApi.Common.Validation;

using EventDescription = Events.Domain.Events.Description;
using EventEndTime = Events.Domain.Events.EndTime;
using EventEntity = Events.Domain.Events.Event;
using EventLocation = Events.Domain.Events.Location;
using EventName = Events.Domain.Events.Name;
using EventStartTime = Events.Domain.Events.StartTime;

namespace Events.WebApi.Events;

public sealed class Event
{
    public static Event FromEntity(EventEntity @event) =>
        new()
        {
            Id = @event.Id.ToString(),
            Name = @event.Name.ToString(),
            Description = @event.Description.ToString(),
            Location = @event.Location.ToString(),
            StartTime = @event.StartTime.Value,
            EndTime = @event.EndTime.Value
        };

    public string? Id { get; set; }

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

    public EventEntity AsEntity()
    {
        EventStartTime startTime = EventStartTime.Of(StartTime, TimeProvider.System);
        return EventEntity.New
        (
            new EventName(Name),
            new EventDescription(Description),
            new EventLocation(Location),
            startTime,
            EventEndTime.Of(EndTime, startTime)
        );
    }
}