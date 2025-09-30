using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzuriteTestContainer;
public sealed class TestWorker(IHostApplicationLifetime appLifetime, IServiceScopeFactory serviceScopeFactory)
    : IHostedService
{
    private Task? _applicationTask;

    // From https://github.com/dfederm/GenericHostConsoleApp/blob/main/ConsoleHostedService.cs
    public Task StartAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource? cancellationTokenSource = null;
        appLifetime.ApplicationStarted.Register(() =>
        {
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _applicationTask = Task.Run(async () =>
            {
                try
                {
                    using var serviceScope = serviceScopeFactory.CreateScope();
                    var job = serviceScope.ServiceProvider.GetRequiredService<IJob>();
                    await job.Archive().ConfigureAwait(false);
                }
                finally
                {
                    // Stop the application once the work is done
                    appLifetime.StopApplication();
                }
            });

        });
        appLifetime.ApplicationStopping.Register(() =>
        {
            cancellationTokenSource?.Cancel();
        });
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Wait for the application logic to fully complete any cleanup tasks.
        // Note that this relies on the cancellation token to be properly used in the application.
        if (_applicationTask != null)
        {
            await _applicationTask.ConfigureAwait(false);
        }
    }
}
