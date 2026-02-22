using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Events.Domain;
using Events.Domain.Events;
using Events.Domain.Events.Registrations;

using RegistrationEmailAddress = Events.Domain.Events.Registrations.EmailAddress;
using RegistrationEntity = Events.Domain.Events.Registrations.Registration;
using RegistrationName = Events.Domain.Events.Registrations.Name;
using RegistrationPhoneNumber = Events.Domain.Events.Registrations.PhoneNumber;

namespace Events.WebApi.Events.Registrations;

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