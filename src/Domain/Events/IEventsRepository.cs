namespace Events.Domain.Events;

public interface IEventsRepository
{
    Task<Event?> Get(Id id, CancellationToken cancellationToken);
    Task Save(Event @event, CancellationToken cancellationToken);
}