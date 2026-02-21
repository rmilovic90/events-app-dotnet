using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Events.Domain;
using Events.Domain.Events;
using Events.WebApi.Common.Validation;

using EventDescription = Events.Domain.Events.Description;
using EventEndTime = Events.Domain.Events.EndTime;
using EventEntity = Events.Domain.Events.Event;
using EventLocation = Events.Domain.Events.Location;
using EventName = Events.Domain.Events.Name;
using EventStartTime = Events.Domain.Events.StartTime;
using RegistrationEntity = Events.Domain.Events.Registration;

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
        EventStartTime startTime = EventStartTime.New(StartTime, TimeProvider.System);
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

public sealed class Registration
{
    public static Registration FromEntity(RegistrationEntity registration) =>
        new()
        {
            Id = registration.Id.ToString(),
            EventId = registration.EventId.ToString(),
            Name = registration.Name.ToString(),
            PhoneNumber = registration.PhoneNumber.ToString(),
            EmailAddress = registration.EmailAddress.ToString()
        };

    public string? Id { get; set; }

    [Description("ID of the event for which registration is made.")]
    public string? EventId { get; set; }

    [Required]
    [MaxLength(RegistrationName.MaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [RegularExpression(RegistrationPhoneNumber.FormatPattern)]
    [Description("Phone number in international (E.164) format (e.g. +38155555555).")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [MaxLength(RegistrationEmailAddress.MaxLength)]
    [EmailAddress]
    public string EmailAddress { get; set; } = null!;

    public RegistrationEntity AsEntity(string eventId) =>
        RegistrationEntity.New
        (
            new Id(eventId),
            new RegistrationName(Name),
            new RegistrationPhoneNumber(PhoneNumber),
            new RegistrationEmailAddress(EmailAddress)
        );
}