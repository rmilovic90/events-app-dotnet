using Events.Domain;
using Events.Domain.Events;

using Npgsql;

using NpgsqlTypes;

using EventEntity = Events.Domain.Events.Event;

namespace Events.WebApi.Events;

internal sealed class Repository : IRepository
{
    public const string EventsTableName = "events";
    public const string EventsTableIdColumnName = "id";
    public const string EventsTableNameColumnName = "name";
    public const string EventsTableDescriptionColumnName = "description";
    public const string EventsTableLocationColumnName = "location";
    public const string EventsTableStartTimeColumnName = "start_time";
    public const string EventsTableStartTimeOffsetColumnName = "start_time_offset";
    public const string EventsTableEndTimeColumnName = "end_time";
    public const string EventsTableEndTimeOffsetColumnName = "end_time_offset";

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
        command.CommandText = $"SELECT * FROM {EventsTableName}";

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
        command.CommandText = $"SELECT * FROM {EventsTableName} WHERE {EventsTableIdColumnName} = @{EventsTableIdColumnName}";
        command.Parameters.AddWithValue(EventsTableIdColumnName, NpgsqlDbType.Uuid, idValue);

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
            $"""
            INSERT INTO {EventsTableName}
            (
                {EventsTableIdColumnName},
                {EventsTableNameColumnName},
                {EventsTableDescriptionColumnName},
                {EventsTableLocationColumnName},
                {EventsTableStartTimeColumnName},
                {EventsTableStartTimeOffsetColumnName},
                {EventsTableEndTimeColumnName},
                {EventsTableEndTimeOffsetColumnName}
            )
            VALUES
            (
                @{EventsTableIdColumnName},
                @{EventsTableNameColumnName},
                @{EventsTableDescriptionColumnName},
                @{EventsTableLocationColumnName},
                @{EventsTableStartTimeColumnName},
                @{EventsTableStartTimeOffsetColumnName},
                @{EventsTableEndTimeColumnName},
                @{EventsTableEndTimeOffsetColumnName}
            )
            ON CONFLICT ({EventsTableIdColumnName})
            DO UPDATE SET
                {EventsTableNameColumnName} = EXCLUDED.{EventsTableNameColumnName},
                {EventsTableDescriptionColumnName} = EXCLUDED.{EventsTableDescriptionColumnName},
                {EventsTableLocationColumnName} = EXCLUDED.{EventsTableLocationColumnName},
                {EventsTableStartTimeColumnName} = EXCLUDED.{EventsTableStartTimeColumnName},
                {EventsTableStartTimeOffsetColumnName} = EXCLUDED.{EventsTableStartTimeOffsetColumnName},
                {EventsTableEndTimeColumnName} = EXCLUDED.{EventsTableEndTimeColumnName},
                {EventsTableEndTimeOffsetColumnName} = EXCLUDED.{EventsTableEndTimeOffsetColumnName}
            """
        );
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableIdColumnName, NpgsqlDbType.Uuid, Guid.Parse(@event.Id.ToString()));
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableNameColumnName, NpgsqlDbType.Varchar, @event.Name.ToString());
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableDescriptionColumnName, NpgsqlDbType.Varchar, @event.Description.ToString());
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableLocationColumnName, NpgsqlDbType.Varchar, @event.Location.ToString());
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableStartTimeColumnName, NpgsqlDbType.Timestamp, @event.StartTime.Value.DateTime);
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableStartTimeOffsetColumnName, NpgsqlDbType.Interval, @event.StartTime.Value.Offset);
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableEndTimeColumnName, NpgsqlDbType.Timestamp, @event.EndTime.Value.DateTime);
        saveOrUpdateEventCommand.Parameters.AddWithValue(EventsTableEndTimeOffsetColumnName, NpgsqlDbType.Interval, @event.EndTime.Value.Offset);

        await saveOrUpdateEventCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    private static EventEntity GetEvent(NpgsqlDataReader reader) =>
        EventEntity.Of
        (
            new Id(reader.GetGuid(reader.GetOrdinal(EventsTableIdColumnName)).ToString()),
            new Name(reader.GetString(reader.GetOrdinal(EventsTableNameColumnName))),
            new Description(reader.GetString(reader.GetOrdinal(EventsTableDescriptionColumnName))),
            new Location(reader.GetString(reader.GetOrdinal(EventsTableLocationColumnName))),
            StartTime.Of
            (
                new DateTimeOffset
                (
                    reader.GetDateTime(reader.GetOrdinal(EventsTableStartTimeColumnName)),
                    reader.GetTimeSpan(reader.GetOrdinal(EventsTableStartTimeOffsetColumnName))
                )
            ),
            EndTime.Of
            (
                new DateTimeOffset
                (
                    reader.GetDateTime(reader.GetOrdinal(EventsTableEndTimeColumnName)),
                    reader.GetTimeSpan(reader.GetOrdinal(EventsTableEndTimeOffsetColumnName))
                )
            )
        );
}