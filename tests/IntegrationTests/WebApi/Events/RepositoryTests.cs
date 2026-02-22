using Events.Domain;

using Npgsql;

using Testcontainers.PostgreSql;
using Testcontainers.Xunit;

using static Events.WebApi.Common.Events.EventEntityBuilder;
using static Events.WebApi.Common.Events.Registrations.RegistrationEntityBuilder;

using EventEntity = Events.Domain.Events.Event;
using RegistrationEntity = Events.Domain.Events.Registration;

namespace Events.WebApi.Events;

public sealed class RepositoryTests(ITestOutputHelper testOutputHelper) : ContainerTest<PostgreSqlBuilder, PostgreSqlContainer>(testOutputHelper)
{
    [Fact]
    public async Task ReturnsAllSavedEvents()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        EventEntity firstEvent = ANewEventEntity
            .WithName("Test 1")
            .WithDescription("Test event 1.")
            .WithLocation("Novi Sad, Serbia")
            .Build();
        EventEntity secondEvent = ANewEventEntity
            .WithName("Test 2")
            .WithDescription("Test event 2.")
            .WithLocation("Novi Sad, Serbia")
            .Build();

        Repository repository = new(Container.GetConnectionString());

        await repository.Save(firstEvent, TestContext.Current.CancellationToken);
        await repository.Save(secondEvent, TestContext.Current.CancellationToken);

        IReadOnlyList<EventEntity> events = await repository.GetAll(TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new[] { firstEvent, secondEvent },
            events
        );
    }

    [Fact]
    public async Task DoesNotFindEvent_WhenEventWithGivenIdDoesNotExist()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity? @event = await repository.Get(new Id(), TestContext.Current.CancellationToken);

        Assert.Null(@event);
    }

    [Fact]
    public async Task FindsEvent_WhenEventWithGivenIdExists()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity eventToSave = ANewEventEntity.Build();

        await repository.Save(eventToSave, TestContext.Current.CancellationToken);

        EventEntity? @event = await repository.Get(eventToSave.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(@event);
        Assert.Multiple
        (
            () => Assert.Equal(eventToSave.Id, @event.Id),
            () => Assert.Equal(eventToSave.Name, @event.Name),
            () => Assert.Equal(eventToSave.Description, @event.Description),
            () => Assert.Equal(eventToSave.Location, @event.Location),
            () => Assert.Equal(eventToSave.StartTime, @event.StartTime),
            () => Assert.Equal(eventToSave.EndTime, @event.EndTime)
        );
    }

    [Fact]
    public async Task ReturnsAllEventRegistrations()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        EventEntity @event = ANewEventEntity.Build();

        Repository repository = new(Container.GetConnectionString());

        await repository.Save(@event, TestContext.Current.CancellationToken);

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

        @event.Add(firstEventRegistration);
        @event.Add(secondEventRegistration);

        await repository.Save(@event, TestContext.Current.CancellationToken);

        IReadOnlyList<RegistrationEntity> eventRegistrations = await repository.GetAllRegistrations(@event.Id, TestContext.Current.CancellationToken);

        Assert.Equivalent
        (
            new[] { firstEventRegistration, secondEventRegistration },
            eventRegistrations
        );
    }

    [Fact]
    public async Task SavesNewEvent()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity eventToSave = ANewEventEntity.Build();

        await repository.Save(eventToSave, TestContext.Current.CancellationToken);

        using NpgsqlDataReader reader = await GetEvent(dataSource, eventToSave);
        await reader.ReadAsync(TestContext.Current.CancellationToken);

        Assert.True(reader.HasRows);
        Assert.Multiple
        (
            () => Assert.Equal(eventToSave.Id.ToString(), reader.GetGuid(reader.GetOrdinal("id")).ToString()),
            () => Assert.Equal(eventToSave.Name.ToString(), reader.GetString(reader.GetOrdinal("name"))),
            () => Assert.Equal(eventToSave.Description.ToString(), reader.GetString(reader.GetOrdinal("description"))),
            () => Assert.Equal(eventToSave.Location.ToString(), reader.GetString(reader.GetOrdinal("location"))),
            () => Assert.Equal(eventToSave.StartTime.Value.DateTime, reader.GetDateTime(reader.GetOrdinal("start_time"))),
            () => Assert.Equal(eventToSave.StartTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal("start_time_offset"))),
            () => Assert.Equal(eventToSave.EndTime.Value.DateTime, reader.GetDateTime(reader.GetOrdinal("end_time"))),
            () => Assert.Equal(eventToSave.EndTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal("end_time_offset")))
        );
    }

    [Fact]
    public async Task SavesRegistrationWithExistingEvent()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity @event = ANewEventEntity.Build();

        await repository.Save(@event, TestContext.Current.CancellationToken);

        RegistrationEntity addedRegistration = ANewRegistrationEntity
            .WithEventId(@event.Id)
            .Build();

        @event.Add(addedRegistration);

        await repository.Save(@event, TestContext.Current.CancellationToken);

        using NpgsqlDataReader reader = await GetEventRegistrations(dataSource, @event);
        await reader.ReadAsync(TestContext.Current.CancellationToken);

        Assert.True(reader.HasRows);
        Assert.Multiple
        (
            () => Assert.Equal(addedRegistration.Id.ToString(), reader.GetGuid(reader.GetOrdinal("id")).ToString()),
            () => Assert.Equal(addedRegistration.EventId.ToString(), reader.GetGuid(reader.GetOrdinal("event_id")).ToString()),
            () => Assert.Equal(addedRegistration.Name.ToString(), reader.GetString(reader.GetOrdinal("name"))),
            () => Assert.Equal(addedRegistration.PhoneNumber.ToString(), reader.GetString(reader.GetOrdinal("phone_number"))),
            () => Assert.Equal(addedRegistration.EmailAddress.ToString(), reader.GetString(reader.GetOrdinal("email_address")))
        );
    }

    protected override PostgreSqlBuilder Configure() => new("postgres:latest");

    private static async Task SetupDatabase(NpgsqlDataSource dataSource)
    {
        using NpgsqlCommand command = dataSource.CreateCommand();
        command.CommandText = await File.ReadAllTextAsync("setup-database.sql", TestContext.Current.CancellationToken);

        await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    private static Task<NpgsqlDataReader> GetEvent(NpgsqlDataSource dataSource, EventEntity @event)
    {
        using NpgsqlCommand getEventByIdQuery = dataSource.CreateCommand();
        getEventByIdQuery.CommandText = $"SELECT * FROM events WHERE id = '{@event.Id}'";

        return getEventByIdQuery.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }

    private static Task<NpgsqlDataReader> GetEventRegistrations(NpgsqlDataSource dataSource, EventEntity @event)
    {
        using NpgsqlCommand getRegistrationsByEventIdQuery = dataSource.CreateCommand();
        getRegistrationsByEventIdQuery.CommandText = $"SELECT * FROM registrations WHERE event_id = '{@event.Id}'";

        return getRegistrationsByEventIdQuery.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }
}