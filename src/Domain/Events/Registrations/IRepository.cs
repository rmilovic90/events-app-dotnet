namespace Events.Domain.Events.Registrations;

public interface IRepository
{
    Task<IReadOnlyList<Registration>> GetAll(Id eventId, CancellationToken cancellationToken);
    Task Save(Registration registration, CancellationToken cancellationToken);
}