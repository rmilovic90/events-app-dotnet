using Events.Domain;
using Events.Domain.Events.Registrations;

using Npgsql;

using NpgsqlTypes;

using RegistrationEntity = Events.Domain.Events.Registrations.Registration;

namespace Events.WebApi.Events.Registrations;

internal sealed class Repository : IRepository
{
    private readonly string _connectionString;

    public Repository(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<RegistrationEntity>> GetAll(Id eventId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(eventId.ToString(), out Guid idValue)) return [];

        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_connectionString);

        using NpgsqlCommand command = dataSource.CreateCommand();
        command.CommandText = "SELECT * FROM registrations WHERE event_id = @eventId";
        command.Parameters.AddWithValue("eventId", NpgsqlDbType.Uuid, idValue);

        NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!reader.HasRows) return [];

        List<RegistrationEntity> registrations = [];
        while (await reader.ReadAsync(cancellationToken))
        {
            registrations.Add
            (
                RegistrationEntity.Of
                (
                    new Id(reader.GetGuid(reader.GetOrdinal("id")).ToString()),
                    new Id(reader.GetGuid(reader.GetOrdinal("event_id")).ToString()),
                    new Name(reader.GetString(reader.GetOrdinal("name"))),
                    new PhoneNumber(reader.GetString(reader.GetOrdinal("phone_number"))),
                    new EmailAddress(reader.GetString(reader.GetOrdinal("email_address")))
                )
            );
        }

        return registrations;
    }

    public async Task Save(RegistrationEntity registration, CancellationToken cancellationToken)
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_connectionString);

        using NpgsqlCommand saveOrUpdateRegistrationsCommand = dataSource.CreateCommand
        (
            """
            INSERT INTO registrations (id, event_id, name, phone_number, email_address)
            VALUES (@id, @event_id, @name, @phone_number, @email_address)
            ON CONFLICT (id)
            DO UPDATE SET
                name = EXCLUDED.name,
                phone_number = EXCLUDED.phone_number,
                email_address = EXCLUDED.email_address
            """
        );
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, Guid.Parse(registration.Id.ToString()));
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue("event_id", NpgsqlDbType.Uuid, Guid.Parse(registration.EventId.ToString()));
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, registration.Name.ToString());
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue("phone_number", NpgsqlDbType.Varchar, registration.PhoneNumber.ToString());
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue("email_address", NpgsqlDbType.Varchar, registration.EmailAddress.ToString());

        await saveOrUpdateRegistrationsCommand.ExecuteNonQueryAsync(cancellationToken);
    }
}