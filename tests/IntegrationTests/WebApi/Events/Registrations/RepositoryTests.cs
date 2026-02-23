using Npgsql;

using Testcontainers.PostgreSql;
using Testcontainers.Xunit;

using static Events.Domain.Events.EventEntityBuilder;
using static Events.Domain.Events.Registrations.RegistrationEntityBuilder;
using static Events.WebApi.Events.Registrations.Repository;

using EventEntity = Events.Domain.Events.Event;
using EventsRepository = Events.WebApi.Events.Repository;
using RegistrationEntity = Events.Domain.Events.Registrations.Registration;
using RegistrationsRepository = Events.WebApi.Events.Registrations.Repository;

namespace Events.WebApi.Events.Registrations;

public sealed class RepositoryTests(ITestOutputHelper testOutputHelper) : ContainerTest<PostgreSqlBuilder, PostgreSqlContainer>(testOutputHelper)
{
    [Fact]
    public async Task ReturnsAllSavedRegistrations()
    {
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        EventEntity @event = ANewEventEntity.Build();

        EventsRepository eventsRepository = new(Container.GetConnectionString());

        await eventsRepository.Save(@event, TestContext.Current.CancellationToken);

        RegistrationEntity firstEventRegistration = ANewRegistrationEntity
            .WithEventId(@event.Id)
            .WithName("Jane Doe")
            .WithPhoneNumber("+38155555555")
            .WithEmailAddress("jane.doe@email.com")
            .Build();
        RegistrationEntity secondEventRegistration = ANewRegistrationEntity
            .WithEventId(@event.Id)
            .WithName("John Doe")
            .WithPhoneNumber("+38155666666")
            .WithEmailAddress("john.doe@email.com")
            .Build();

        RegistrationsRepository registrationsRepository = new(Container.GetConnectionString());

        await registrationsRepository.Save(firstEventRegistration, TestContext.Current.CancellationToken);
        await registrationsRepository.Save(secondEventRegistration, TestContext.Current.CancellationToken);

        IReadOnlyList<RegistrationEntity> eventRegistrations = await registrationsRepository.GetAll(@event.Id, TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new[] { firstEventRegistration, secondEventRegistration },
            eventRegistrations
        );
    }

    [Fact]
    public async Task SavesNewRegistration()
    {
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        EventsRepository eventsRepository = new(Container.GetConnectionString());

        EventEntity @event = ANewEventEntity.Build();

        await eventsRepository.Save(@event, TestContext.Current.CancellationToken);

        RegistrationEntity addedRegistration = ANewRegistrationEntity
            .WithEventId(@event.Id)
            .Build();

        RegistrationsRepository registrationsRepository = new(Container.GetConnectionString());

        await registrationsRepository.Save(addedRegistration, TestContext.Current.CancellationToken);

        await using NpgsqlDataReader reader = await GetEventRegistrations(dataSource, @event);
        await reader.ReadAsync(TestContext.Current.CancellationToken);

        Assert.True(reader.HasRows);
        Assert.Multiple
        (
            () => Assert.Equal(addedRegistration.Id.ToString(), reader.GetGuid(reader.GetOrdinal(RegistrationsTableIdColumnName)).ToString()),
            () => Assert.Equal(addedRegistration.EventId.ToString(), reader.GetGuid(reader.GetOrdinal(RegistrationsTableEventIdColumnName)).ToString()),
            () => Assert.Equal(addedRegistration.Name.ToString(), reader.GetString(reader.GetOrdinal(RegistrationsTableNameColumnName))),
            () => Assert.Equal(addedRegistration.PhoneNumber.ToString(), reader.GetString(reader.GetOrdinal(RegistrationsTablePhoneNumberColumnName))),
            () => Assert.Equal(addedRegistration.EmailAddress.ToString(), reader.GetString(reader.GetOrdinal(RegistrationsTableEmailAddressColumnName)))
        );
    }

    protected override PostgreSqlBuilder Configure() => new("postgres:latest");

    private static async Task SetupDatabase(NpgsqlDataSource dataSource)
    {
        await using NpgsqlCommand command = dataSource.CreateCommand();
        command.CommandText = await File.ReadAllTextAsync("setup-database.sql", TestContext.Current.CancellationToken);

        await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    private static async Task<NpgsqlDataReader> GetEventRegistrations(NpgsqlDataSource dataSource, EventEntity @event)
    {
        await using NpgsqlCommand getRegistrationsByEventIdQuery = dataSource.CreateCommand();
        getRegistrationsByEventIdQuery.CommandText = $"SELECT * FROM {RegistrationsTableName} WHERE {RegistrationsTableEventIdColumnName} = '{@event.Id}'";

        return await getRegistrationsByEventIdQuery.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }
}