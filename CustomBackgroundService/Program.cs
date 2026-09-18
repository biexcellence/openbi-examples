using BiExcellence.OpenBi.Server.License.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CustomBackgroundServiceExample;

// An IHostedService is started with the open bi server and stopped when it shuts down.
// For work which an administrator should be able to configure and schedule, use a batch job
// handler instead, see the CustomBatchJobHandler example.
public sealed class CustomBackgroundService : BackgroundService
{
    private readonly ILicense _license;
    private readonly ILogger<CustomBackgroundService> _logger;

    public CustomBackgroundService(ILicense license, ILogger<CustomBackgroundService> logger)
    {
        _license = license;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background service of {LicenseName} started", _license.Name);

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Doing some work");
            }
        }
        catch (OperationCanceledException)
        {
            // the server is shutting down
        }

        _logger.LogInformation("Background service stopped");
    }
}
