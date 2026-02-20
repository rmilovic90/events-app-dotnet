namespace Events.Domain.Events;

public sealed class Event
{
    public static Event New
    (
        Name name,
        Description description,
        Location location,
        StartTime startTime,
        EndTime endTime
    ) => Of
    (
        new Id(),
        name,
        description,
        location,
        startTime,
        endTime
    );

    public static Event Of
    (
        Id id,
        Name name,
        Description description,
        Location location,
        StartTime startTime,
        EndTime endTime
    ) => new
    (
        id,
        name,
        description,
        location,
        startTime,
        endTime
    );

    private readonly List<Registration> _pendingRegistrations = [];

    private Event
    (
        Id id,
        Name name,
        Description description,
        Location location,
        StartTime startTime,
        EndTime endTime
    )
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(location);
        ArgumentNullException.ThrowIfNull(startTime);
        ArgumentNullException.ThrowIfNull(endTime);

        Id = id;
        Name = name;
        Description = description;
        Location = location;
        StartTime = startTime;
        EndTime = endTime;
    }

    public Id Id { get; }
    public Name Name { get; }
    public Description Description { get; }
    public Location Location { get; }
    public StartTime StartTime { get; }
    public EndTime EndTime { get; }

    public IReadOnlyList<Registration> PendingRegistrations => _pendingRegistrations;

    public void Add(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        _pendingRegistrations.Add(registration);
    }

    public override string ToString() =>
        $"{nameof(Event)} {{ {nameof(Id)} = {Id}, {nameof(Name)} = {Name}, {nameof(Description)} = {Description}, {nameof(Location)} = {Location}, {nameof(StartTime)} = {StartTime}, {nameof(EndTime)} = {EndTime} }}";
}