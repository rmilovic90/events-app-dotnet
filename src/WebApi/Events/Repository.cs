using Events.Domain.Events;

using Npgsql;

using NpgsqlTypes;

namespace Events.WebApi.Events;

internal sealed class Repository : IEventsRepository
{
    private readonly string _connectionString;

    public Repository(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        _connectionString = connectionString;
    }

    public async Task Save(Domain.Events.Event @event, CancellationToken cancellationToken)
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_connectionString);

        using NpgsqlCommand command = dataSource.CreateCommand();
        command.CommandText =
        """
        INSERT INTO events("id", "name", "description", "location", "start_time", "start_time_offset", "end_time", "end_time_offset")
            VALUES (@id, @name, @description, @location, @start_time, @start_time_offset, @end_time, @end_time_offset)
        """;
        command.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Uuid) { Value = Guid.Parse(@event.Id.ToString()) });
        command.Parameters.Add(new NpgsqlParameter("name", NpgsqlDbType.Varchar) { Value = @event.Name.ToString() });
        command.Parameters.Add(new NpgsqlParameter("description", NpgsqlDbType.Varchar) { Value = @event.Description.ToString() });
        command.Parameters.Add(new NpgsqlParameter("location", NpgsqlDbType.Varchar) { Value = @event.Location.ToString() });
        command.Parameters.Add(new NpgsqlParameter("start_time", NpgsqlDbType.TimestampTz) { Value = @event.StartTime.Value.ToUniversalTime() });
        command.Parameters.Add(new NpgsqlParameter("start_time_offset", NpgsqlDbType.Interval) { Value = @event.StartTime.Value.Offset });
        command.Parameters.Add(new NpgsqlParameter("end_time", NpgsqlDbType.TimestampTz) { Value = @event.EndTime.Value.ToUniversalTime() });
        command.Parameters.Add(new NpgsqlParameter("end_time_offset", NpgsqlDbType.Interval) { Value = @event.EndTime.Value.Offset });

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}