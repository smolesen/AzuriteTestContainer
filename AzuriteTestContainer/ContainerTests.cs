using AzuriteTestContainer.Fixture;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AzuriteTestContainer;
[Collection(nameof(SharedTestCollection))]
public sealed class ContainerTest(TestContainersFixture testContainersFixture, ITestOutputHelper output)
{
    private IHost ArrangeHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var initialData = new List<KeyValuePair<string, string?>>
                {
                    new("ConnectionStrings:StorageAccount",testContainersFixture.StorageConnectionString),
                };
                config.AddInMemoryCollection(initialData);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(output, appendScope: false));
                services.AddSingleton<IJob, Job>();
                services.AddAzureClients(c =>
                {
                    var archiveStorageAccountConnectionString = configuration.GetConnectionString("StorageAccount");
                    c.AddBlobServiceClient(archiveStorageAccountConnectionString).WithName("ArchiveStorage");
                });

                services.AddHostedService<TestWorker>();

            }).Build();
    }

    [Fact]
    public async Task Archive()
    {
        // Arrange the host
        var cts = new CancellationTokenSource();
        var host = ArrangeHost();

        await host.StartAsync(cts.Token);

        // Act
        // This is where the job is really executed
        await host.StopAsync(cts.Token);
    }
}
