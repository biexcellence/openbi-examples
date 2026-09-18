using BiExcellence.OpenBi.Server.License.Abstractions;
using Ibssolution.biox.Repositoryserver;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace CustomCommandHandlerExample;

// Adds a command to the legacy command API (/openbi/xmlprovider/mobile.biex).
// New functionality should be added as a REST endpoint instead, see the CustomHttpHandler example.
public sealed class CustomCommandHandler : ICommandApiHandler
{
    private readonly ILicense _license;
    private readonly ILogger<CustomCommandHandler> _logger;

    // Services are injected through the constructor
    public CustomCommandHandler(ILicense license, ILogger<CustomCommandHandler> logger)
    {
        _license = license;
        _logger = logger;
    }

    public Task<bool> ProcessCommandAsync(JsonObject responseObj, sxRequest request, OpenBiSession session, CancellationToken cancellationToken)
    {
        // Every handler is asked for every command, so return false for all other commands
        if (request.Command != "MY_CUSTOM_COMMAND")
        {
            return Task.FromResult(false);
        }

        _logger.LogInformation("Processing {Command}", request.Command);

        // Read a parameter sent by the client
        if (request.Parameters.TryGetValue("PARAMETER", out var nameValue))
        {
            responseObj["RETURN"] = nameValue.Value;
        }

        // Check whether the session is authenticated
        if (session.User is not null)
        {
            responseObj["USERNAME"] = session.User.Username;
        }

        responseObj["LICENSE_NAME"] = _license.Name;

        // Nested objects and arrays are written with JsonObject / JsonArray
        responseObj["VALUES"] = new JsonArray("first", "second");

        // Throw an OpenbiMobileCommandException to answer with a specific return code
        if (request.Parameters.ContainsKey("FAIL"))
        {
            throw new OpenbiMobileCommandException("Something went wrong", -100);
        }

        // Mark the command as handled
        return Task.FromResult(true);
    }
}
