namespace Events.Domain.Events;

public interface IEventsRepository
{
    Task Save(Event @event, CancellationToken cancellationToken);
}