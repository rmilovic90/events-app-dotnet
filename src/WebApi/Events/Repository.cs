using Events.Domain;
using Events.Domain.Events;

using Npgsql;

using NpgsqlTypes;

using EventEntity = Events.Domain.Events.Event;

namespace Events.WebApi.Events;

internal sealed class Repository : IRepository
{
    private readonly string _connectionString;

    public Repository(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<EventEntity>> GetAll(CancellationToken cancellationToken)
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_connectionString);

        using NpgsqlCommand command = dataSource.CreateCommand();
        command.CommandText = "SELECT * FROM events";

        NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!reader.HasRows) return [];

        List<EventEntity> events = [];
        while (await reader.ReadAsync(cancellationToken))
        {
            events.Add(GetEvent(reader));
        }

        return events;
    }

    public async Task<EventEntity?> Get(Id id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id.ToString(), out Guid idValue)) return null;

        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_connectionString);

        using NpgsqlCommand command = dataSource.CreateCommand();
        command.CommandText = "SELECT * FROM events WHERE id = @id";
        command.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, idValue);

        NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!reader.HasRows) return null;

        await reader.ReadAsync(cancellationToken);

        return GetEvent(reader);
    }

    public async Task Save(EventEntity @event, CancellationToken cancellationToken)
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_connectionString);

        using NpgsqlCommand saveOrUpdateEventCommand = dataSource.CreateCommand
        (
            """
            INSERT INTO events (id, name, description, location, start_time, start_time_offset, end_time, end_time_offset)
            VALUES (@id, @name, @description, @location, @start_time, @start_time_offset, @end_time, @end_time_offset)
            ON CONFLICT (id)
            DO UPDATE SET
                name = EXCLUDED.name,
                description = EXCLUDED.description,
                location = EXCLUDED.location,
                start_time = EXCLUDED.start_time,
                start_time_offset = EXCLUDED.start_time_offset,
                end_time = EXCLUDED.end_time,
                end_time_offset = EXCLUDED.end_time_offset
            """
        );
        saveOrUpdateEventCommand.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, Guid.Parse(@event.Id.ToString()));
        saveOrUpdateEventCommand.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, @event.Name.ToString());
        saveOrUpdateEventCommand.Parameters.AddWithValue("description", NpgsqlDbType.Varchar, @event.Description.ToString());
        saveOrUpdateEventCommand.Parameters.AddWithValue("location", NpgsqlDbType.Varchar, @event.Location.ToString());
        saveOrUpdateEventCommand.Parameters.AddWithValue("start_time", NpgsqlDbType.Timestamp, @event.StartTime.Value.DateTime);
        saveOrUpdateEventCommand.Parameters.AddWithValue("start_time_offset", NpgsqlDbType.Interval, @event.StartTime.Value.Offset);
        saveOrUpdateEventCommand.Parameters.AddWithValue("end_time", NpgsqlDbType.Timestamp, @event.EndTime.Value.DateTime);
        saveOrUpdateEventCommand.Parameters.AddWithValue("end_time_offset", NpgsqlDbType.Interval, @event.EndTime.Value.Offset);

        await saveOrUpdateEventCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    private static EventEntity GetEvent(NpgsqlDataReader reader) =>
        EventEntity.Of
        (
            new Id(reader.GetGuid(reader.GetOrdinal("id")).ToString()),
            new Name(reader.GetString(reader.GetOrdinal("name"))),
            new Description(reader.GetString(reader.GetOrdinal("description"))),
            new Location(reader.GetString(reader.GetOrdinal("location"))),
            StartTime.Of
            (
                new DateTimeOffset
                (
                    reader.GetDateTime(reader.GetOrdinal("start_time")),
                    reader.GetTimeSpan(reader.GetOrdinal("start_time_offset"))
                )
            ),
            EndTime.Of
            (
                new DateTimeOffset
                (
                    reader.GetDateTime(reader.GetOrdinal("end_time")),
                    reader.GetTimeSpan(reader.GetOrdinal("end_time_offset"))
                )
            )
        );
}