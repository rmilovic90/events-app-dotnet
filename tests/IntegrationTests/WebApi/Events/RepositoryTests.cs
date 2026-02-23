using Events.Domain;

using Npgsql;

using Testcontainers.PostgreSql;
using Testcontainers.Xunit;

using static Events.Domain.Events.EventEntityBuilder;
using static Events.WebApi.Events.Repository;

using EventEntity = Events.Domain.Events.Event;

namespace Events.WebApi.Events;

public sealed class RepositoryTests(ITestOutputHelper testOutputHelper) : ContainerTest<PostgreSqlBuilder, PostgreSqlContainer>(testOutputHelper)
{
    [Fact]
    public async Task ReturnsAllSavedEvents()
    {
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

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
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity? @event = await repository.Get(new Id(), TestContext.Current.CancellationToken);

        Assert.Null(@event);
    }

    [Fact]
    public async Task FindsEvent_WhenEventWithGivenIdExists()
    {
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

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
    public async Task SavesNewEvent()
    {
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity eventToSave = ANewEventEntity.Build();

        await repository.Save(eventToSave, TestContext.Current.CancellationToken);

        await using NpgsqlDataReader reader = await GetEvent(dataSource, eventToSave);
        await reader.ReadAsync(TestContext.Current.CancellationToken);

        Assert.True(reader.HasRows);
        Assert.Multiple
        (
            () => Assert.Equal(eventToSave.Id.ToString(), reader.GetGuid(reader.GetOrdinal(EventsTableIdColumnName)).ToString()),
            () => Assert.Equal(eventToSave.Name.ToString(), reader.GetString(reader.GetOrdinal(EventsTableNameColumnName))),
            () => Assert.Equal(eventToSave.Description.ToString(), reader.GetString(reader.GetOrdinal(EventsTableDescriptionColumnName))),
            () => Assert.Equal(eventToSave.Location.ToString(), reader.GetString(reader.GetOrdinal(EventsTableLocationColumnName))),
            () => Assert.Equal(eventToSave.StartTime.Value.DateTime, reader.GetDateTime(reader.GetOrdinal(EventsTableStartTimeColumnName))),
            () => Assert.Equal(eventToSave.StartTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal(EventsTableStartTimeOffsetColumnName))),
            () => Assert.Equal(eventToSave.EndTime.Value.DateTime, reader.GetDateTime(reader.GetOrdinal(EventsTableEndTimeColumnName))),
            () => Assert.Equal(eventToSave.EndTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal(EventsTableEndTimeOffsetColumnName)))
        );
    }

    protected override PostgreSqlBuilder Configure() => new("postgres:latest");

    private static async Task SetupDatabase(NpgsqlDataSource dataSource)
    {
        await using NpgsqlCommand command = dataSource.CreateCommand();
        command.CommandText = await File.ReadAllTextAsync("setup-database.sql", TestContext.Current.CancellationToken);

        await command.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    private static async Task<NpgsqlDataReader> GetEvent(NpgsqlDataSource dataSource, EventEntity @event)
    {
        await using NpgsqlCommand getEventByIdQuery = dataSource.CreateCommand();
        getEventByIdQuery.CommandText = $"SELECT * FROM {EventsTableName} WHERE {EventsTableIdColumnName} = '{@event.Id}'";

        return await getEventByIdQuery.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }
}