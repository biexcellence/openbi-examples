using BiExcellence.OpenBi.Server.BatchJob.Abstractions;
using BiExcellence.OpenBi.Server.License.Abstractions;
using Microsoft.Extensions.Logging;

namespace CustomBatchJobHandlerExample;

// The unique code and the description of the job type, both are shown in the Configurator
[BatchJobHandler("CUSTOMBATCHJOBHANDLER", "My Custom Batch Job Handler")]
// The parameters of the job type
[BatchJobHandlerParameter("TEXT", "Custom Text", "default value")]
public sealed class CustomBatchJobHandler : BatchJobHandler
{
    private readonly ILicense _license;

    // Services are injected through the constructor
    public CustomBatchJobHandler(ILicense license)
    {
        _license = license;
    }

    public override async Task RunAsync(IBatchJobHandlerRunContext context, CancellationToken cancellationToken)
    {
        // Everything which is logged into context.Logger becomes part of the job log
        context.Logger.LogInformation("Job: {JobName} ({JobId})", context.Job.Name, context.Job.Id);
        context.Logger.LogInformation("User: {Username}", context.User.Identity?.Name);
        context.Logger.LogInformation("License: {LicenseName}", _license.Name);

        // Read a parameter of the job
        if (context.Parameters.TryGetValue("TEXT", out var text))
        {
            context.Logger.LogInformation("Text: {Text}", text);
        }

        // Long running work has to observe the CancellationToken so the job can be canceled
        for (var i = 0; i < 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            context.Logger.LogDebug("Step {Step} of 5", i + 1);
        }

        // An unhandled exception marks the job as failed
    }

    // Optional: run the job on runtimes which the periodic settings of a job cannot express,
    // here on the last day of the month
    public override IEnumerable<DateTimeOffset> GetCustomRuntimes(IBatchJobHandlerCheckRuntimeContext context, DateTimeOffset now)
    {
        var lastDayOfMonth = new DateTimeOffset(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 2, 0, 0, now.Offset);
        if (lastDayOfMonth > now)
        {
            yield return lastDayOfMonth;
        }
    }
}
