using DotNet.Testcontainers.Builders;
using Testcontainers.Azurite;

namespace AzuriteTestContainer.Fixture;
public sealed class TestContainersFixture : IAsyncLifetime
{
    public AzuriteContainer? AzuriteContainer { get; private set; }
    public string? StorageConnectionString { get; private set; }


    public async Task InitializeAsync()
    {
        var networkName = "test_network";
        var network = new NetworkBuilder()
            .Build();

        // Azurite container
        AzuriteContainer = new AzuriteBuilder()
            .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithNetwork(network)
            .WithNetworkAliases(networkName)
            .Build();


        await Task.WhenAll(AzuriteContainer.StartAsync());

        StorageConnectionString = AzuriteContainer.GetConnectionString();//

    }

    public async Task DisposeAsync()
    {
        var containersToDispose = new List<Task>();
        if (AzuriteContainer is not null)
            containersToDispose.Add(AzuriteContainer.DisposeAsync().AsTask());
        await Task.WhenAll(containersToDispose).ConfigureAwait(false);
    }
}