namespace Events.Domain.Events;

public interface IRepository
{
    Task<IReadOnlyList<Event>> GetAll(CancellationToken cancellationToken);
    Task<Event?> Get(Id id, CancellationToken cancellationToken);
    Task Save(Event @event, CancellationToken cancellationToken);
}