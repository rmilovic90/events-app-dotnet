using Events.Domain;
using Events.Domain.Events;

using Microsoft.Extensions.Time.Testing;

using Npgsql;

using Testcontainers.PostgreSql;
using Testcontainers.Xunit;

using EventEntity = Events.Domain.Events.Event;
using RegistrationEntity = Events.Domain.Events.Registration;

namespace Events.WebApi.Events;

public sealed class RepositoryTests(ITestOutputHelper testOutputHelper) : ContainerTest<PostgreSqlBuilder, PostgreSqlContainer>(testOutputHelper)
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset UtcTomorrow = UtcNow.AddDays(1);
    private static readonly DateTimeOffset UtcDayAfterTomorrow = UtcTomorrow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    private static readonly StartTime StartTime = StartTime.New
    (
        new DateTimeOffset
        (
            UtcTomorrow.Year,
            UtcTomorrow.Month,
            UtcTomorrow.Day,
            UtcTomorrow.Hour,
            UtcTomorrow.Minute,
            UtcTomorrow.Second,
            CentralEuropeanTimeZone.GetUtcOffset(UtcTomorrow)
        ),
        new FakeTimeProvider(UtcNow)
    );
    private static EventEntity Event => EventEntity.New
    (
        new Name("Test"),
        new Description("Test event."),
        new Location("Novi Sad, Serbia"),
        StartTime,
        EndTime.Of
        (
            new DateTimeOffset
            (
                UtcDayAfterTomorrow.Year,
                UtcDayAfterTomorrow.Month,
                UtcDayAfterTomorrow.Day,
                UtcDayAfterTomorrow.Hour,
                UtcDayAfterTomorrow.Minute,
                UtcDayAfterTomorrow.Second,
                CentralEuropeanTimeZone.GetUtcOffset(UtcDayAfterTomorrow)
            ),
            StartTime
        )
    );

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

        EventEntity eventToSave = Event;

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
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity eventToSave = Event;

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
            () => Assert.Equal(eventToSave.StartTime.Value, reader.GetDateTime(reader.GetOrdinal("start_time"))),
            () => Assert.Equal(eventToSave.StartTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal("start_time_offset"))),
            () => Assert.Equal(eventToSave.EndTime.Value, reader.GetDateTime(reader.GetOrdinal("end_time"))),
            () => Assert.Equal(eventToSave.EndTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal("end_time_offset")))
        );
    }

    [Fact]
    public async Task SavesRegistrationWithExistingEvent()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());

        EventEntity @event = Event;

        await repository.Save(@event, TestContext.Current.CancellationToken);

        RegistrationEntity addedRegistration = RegistrationEntity.New
        (
            @event.Id,
            new RegistrationName("Jane Doe"),
            new RegistrationPhoneNumber("+38155555555"),
            new RegistrationEmailAddress("jane.doe@email.com")
        );

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
        getEventByIdQuery.CommandText = @$"SELECT * FROM events WHERE ""id"" = '{@event.Id}'";

        return getEventByIdQuery.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }

    private static Task<NpgsqlDataReader> GetEventRegistrations(NpgsqlDataSource dataSource, EventEntity @event)
    {
        using NpgsqlCommand getRegistrationsByEventIdQuery = dataSource.CreateCommand();
        getRegistrationsByEventIdQuery.CommandText = @$"SELECT * FROM registrations WHERE ""event_id"" = '{@event.Id}'";

        return getRegistrationsByEventIdQuery.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }
}