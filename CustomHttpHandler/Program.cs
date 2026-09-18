using BiExcellence.OpenBi.Server.License.Abstractions;
using Ibssolution.biox.Repositoryserver;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomHttpHandlerExample;

// IOpenBiStartup is called while the open bi server builds its request pipeline.
// It replaces the IAsyncHttpHandler interface of older server versions.
public sealed class CustomHttpHandler : IOpenBiStartup
{
    private readonly ILicense _license;
    private readonly ILogger<CustomHttpHandler> _logger;

    // Services are injected through the constructor
    public CustomHttpHandler(ILicense license, ILogger<CustomHttpHandler> logger)
    {
        _license = license;
        _logger = logger;
    }

    public void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes)
    {
        // An endpoint for a single path
        routes.Map("/mycustomhttphandler", async context =>
        {
            _logger.LogInformation("Processing {Path}", context.Request.Path);

            context.Response.ContentType = "text/html";

            // Check whether the user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                await context.Response.WriteAsync($"<h1>Username: {context.User.Identity.Name}</h1>");
            }

            await context.Response.WriteAsync($"License Name: {_license.Name}");
        });

        // Several endpoints below a common path
        var group = routes.MapGroup("/mycustomhttphandler/api");

        group.Map("/query", static async context =>
        {
            // Scoped services of the request are resolved through RequestServices
            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

            await context.Response.WriteAsJsonAsync(new { HttpPort = configuration[OpenBiConfigurationProvider.HttpPort] });
        });

        // Middleware which runs for every request, e.g. to add a response header
        app.Use(static next => async context =>
        {
            context.Response.Headers["X-Custom-Plugin"] = "Hello World";

            await next(context);
        });
    }
}
