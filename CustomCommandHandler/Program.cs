using BiExcellence.OpenBi.Server.License.Abstractions;
using Ibssolution.biox.Repositoryserver;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomCommandHandlerExample
{
    public class CustomCommandHandler : IAsyncRequestHandler
    {
        private readonly ILicense _license;

        // Get ILicense from DI
        public CustomCommandHandler(ILicense license)
        {
            _license = license;
        }

        public async Task<bool> Process_Command(JObject responseObj, sxRequest request, openBiMobileRequest session, CancellationToken cancellationToken)
        {
            // Check command name
            if (request.Command == "MY_CUSTOM_COMMAND")
            {
                // Get parameter send by the client
                if (request.Parameters.TryGetValue("PARAMETER", out var nameValue))
                {
                    responseObj["RETURN"] = nameValue.Value;
                }

                // Check if the session is authentificated
                if (session.User != null)
                {
                    responseObj["USERNAME"] = session.User.Username;
                }

                responseObj["LICENSE_NAME"] = _license.Name;

                // Mark command as handled
                return true;
            }
            // Command is not handled
            return false;
        }
    }
}