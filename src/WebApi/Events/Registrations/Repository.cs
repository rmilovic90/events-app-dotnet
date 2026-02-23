using Events.Domain;
using Events.Domain.Events.Registrations;

using Npgsql;

using NpgsqlTypes;

using RegistrationEntity = Events.Domain.Events.Registrations.Registration;

namespace Events.WebApi.Events.Registrations;

internal sealed class Repository : IRepository
{
    public const string RegistrationsTableName = "registrations";
    public const string RegistrationsTableIdColumnName = "id";
    public const string RegistrationsTableEventIdColumnName = "event_id";
    public const string RegistrationsTableNameColumnName = "name";
    public const string RegistrationsTablePhoneNumberColumnName = "phone_number";
    public const string RegistrationsTableEmailAddressColumnName = "email_address";

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
        command.CommandText = $"SELECT * FROM {RegistrationsTableName} WHERE {RegistrationsTableEventIdColumnName} = @{RegistrationsTableEventIdColumnName}";
        command.Parameters.AddWithValue(RegistrationsTableEventIdColumnName, NpgsqlDbType.Uuid, idValue);

        NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!reader.HasRows) return [];

        List<RegistrationEntity> registrations = [];
        while (await reader.ReadAsync(cancellationToken))
        {
            registrations.Add
            (
                RegistrationEntity.Of
                (
                    new Id(reader.GetGuid(reader.GetOrdinal(RegistrationsTableIdColumnName)).ToString()),
                    new Id(reader.GetGuid(reader.GetOrdinal(RegistrationsTableEventIdColumnName)).ToString()),
                    new Name(reader.GetString(reader.GetOrdinal(RegistrationsTableNameColumnName))),
                    new PhoneNumber(reader.GetString(reader.GetOrdinal(RegistrationsTablePhoneNumberColumnName))),
                    new EmailAddress(reader.GetString(reader.GetOrdinal(RegistrationsTableEmailAddressColumnName)))
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
            $"""
            INSERT INTO {RegistrationsTableName}
            (
                {RegistrationsTableIdColumnName},
                {RegistrationsTableEventIdColumnName},
                {RegistrationsTableNameColumnName},
                {RegistrationsTablePhoneNumberColumnName},
                {RegistrationsTableEmailAddressColumnName}
            )
            VALUES
            (
                @{RegistrationsTableIdColumnName},
                @{RegistrationsTableEventIdColumnName},
                @{RegistrationsTableNameColumnName},
                @{RegistrationsTablePhoneNumberColumnName},
                @{RegistrationsTableEmailAddressColumnName}
            )
            ON CONFLICT ({RegistrationsTableIdColumnName})
            DO UPDATE SET
                {RegistrationsTableNameColumnName} = EXCLUDED.{RegistrationsTableNameColumnName},
                {RegistrationsTablePhoneNumberColumnName} = EXCLUDED.{RegistrationsTablePhoneNumberColumnName},
                {RegistrationsTableEmailAddressColumnName} = EXCLUDED.{RegistrationsTableEmailAddressColumnName}
            """
        );
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue(RegistrationsTableIdColumnName, NpgsqlDbType.Uuid, Guid.Parse(registration.Id.ToString()));
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue(RegistrationsTableEventIdColumnName, NpgsqlDbType.Uuid, Guid.Parse(registration.EventId.ToString()));
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue(RegistrationsTableNameColumnName, NpgsqlDbType.Varchar, registration.Name.ToString());
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue(RegistrationsTablePhoneNumberColumnName, NpgsqlDbType.Varchar, registration.PhoneNumber.ToString());
        saveOrUpdateRegistrationsCommand.Parameters.AddWithValue(RegistrationsTableEmailAddressColumnName, NpgsqlDbType.Varchar, registration.EmailAddress.ToString());

        await saveOrUpdateRegistrationsCommand.ExecuteNonQueryAsync(cancellationToken);
    }
}