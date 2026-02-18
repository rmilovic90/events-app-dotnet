using Events.Domain.Events;

using Microsoft.Extensions.Time.Testing;

using Npgsql;

using Testcontainers.PostgreSql;
using Testcontainers.Xunit;

using EventEntity = Events.Domain.Events.Event;

namespace Events.WebApi.Events;

public sealed class RepositoryTests(ITestOutputHelper testOutputHelper) : ContainerTest<PostgreSqlBuilder, PostgreSqlContainer>(testOutputHelper)
{
    private static readonly DateTimeOffset UtcNow = DateTimeOffset.UtcNow;
    private static readonly DateTimeOffset UtcTomorrow = UtcNow.AddDays(1);
    private static readonly DateTimeOffset UtcDayAfterTomorrow = UtcTomorrow.AddDays(1);
    private static readonly TimeZoneInfo CentralEuropeanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    private static readonly StartTime StartTime = StartTime.Of
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
    private static readonly EventEntity Event = EventEntity.New
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
    public async Task SavesNewEvent()
    {
        using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Container.GetConnectionString());

        await SetupDatabase(dataSource);

        Repository repository = new(Container.GetConnectionString());
        await repository.Save(Event, TestContext.Current.CancellationToken);

        using NpgsqlDataReader reader = await GetEvent(dataSource, Event);
        await reader.ReadAsync(TestContext.Current.CancellationToken);

        Assert.True(reader.HasRows);
        Assert.Multiple
        (
            () => Assert.Equal(Event.Id.ToString(), reader.GetGuid(reader.GetOrdinal("id")).ToString()),
            () => Assert.Equal(Event.Name.ToString(), reader.GetString(reader.GetOrdinal("name"))),
            () => Assert.Equal(Event.Description.ToString(), reader.GetString(reader.GetOrdinal("description"))),
            () => Assert.Equal(Event.Location.ToString(), reader.GetString(reader.GetOrdinal("location"))),
            () => Assert.Equal(Event.StartTime.Value.ToUniversalTime(), reader.GetDateTime(reader.GetOrdinal("start_time"))),
            () => Assert.Equal(Event.StartTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal("start_time_offset"))),
            () => Assert.Equal(Event.EndTime.Value.ToUniversalTime(), reader.GetDateTime(reader.GetOrdinal("end_time"))),
            () => Assert.Equal(Event.EndTime.Value.Offset, reader.GetTimeSpan(reader.GetOrdinal("end_time_offset")))
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
        getEventByIdQuery.CommandText = @$"SELECT * FROM public.""events"" WHERE ""id"" = '{@event.Id}'";

        return getEventByIdQuery.ExecuteReaderAsync(TestContext.Current.CancellationToken);
    }
}