using BiExcellence.OpenBi.Server.License.Abstractions;
using Ibssolution.biox.Repositoryserver;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CustomBatchJobHandlerExample
{
    public class CustomBatchJobHandler : BatchHandlerBase
    {
        private readonly ILicense _license;

        // Get ILicense from DI
        public CustomBatchJobHandler(ILicense license)
        {
            _license = license;
        }

        public override async Task Run(BatchJob job, CancellationToken cancellationToken)
        {
            // Get text parameter
            job.Parameters.TryGetValue("text", out var text);

            // Log values
            job.LogWriter.LogDebug($"Text: {text}");
            job.LogWriter.LogInformation($"User: {job.User.Username}");
            job.LogWriter.LogWarning($"License Name: {_license.Name}");
        }

        // Document possible parameters
        public override void AddBatchParametersToCollection(BatchParameterCollection collection)
        {
            collection.Add("text", new BatchParameter("text", eBatchParametertype.Text, "Custom Text"));
        }

        // Unique Batch Job Handler code
        public override string HandlerCode()
        {
            return "CUSTOMBATCHJOBHANDLER";
        }

        // Document Batch Job Handler
        public override string HandlerName()
        {
            return "My Custom Batch Job Handler";
        }
    }
}
