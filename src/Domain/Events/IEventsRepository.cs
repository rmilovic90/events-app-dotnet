namespace Events.Domain.Events;

public interface IEventsRepository
{
    Task<IReadOnlyList<Event>> GetAll(CancellationToken cancellationToken);
    Task<Event?> Get(Id id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Registration>> GetAllRegistrations(Id eventId, CancellationToken cancellationToken);
    Task Save(Event @event, CancellationToken cancellationToken);
}