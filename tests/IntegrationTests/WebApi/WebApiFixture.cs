using Events.Domain.Events;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

namespace Events.WebApi;

[CollectionDefinition(Name)]
public sealed class WebApiTestCollection : ICollectionFixture<WebApiFixture>
{
    public const string Name = "Web API Test";
}

public sealed class WebApiFixture : WebApplicationFactory<Program>
{
    internal IEventsRepository EventsRepositoryMock { get; private set; } = Substitute.For<IEventsRepository>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices
        (
            services => services.AddSingleton
            (
                _ =>
                {
                    return EventsRepositoryMock;
                }
            )
        );
    }
}