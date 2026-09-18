using Ibssolution.biox.Repositoryserver;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomPluginExample;

// Registered by CustomPlugin.ConfigureServices, so the service of this plugin can be injected.
// Services of other plugins can be injected as well.
public sealed class CustomStartup : IOpenBiStartup
{
    private readonly ICustomService _customService;
    private readonly ILogger<CustomStartup> _logger;

    public CustomStartup(ICustomService customService, ILogger<CustomStartup> logger)
    {
        _customService = customService;
        _logger = logger;
    }

    public void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes)
    {
        routes.Map("/customplugin", async context =>
        {
            _logger.LogInformation("Processing {Path}", context.Request.Path);

            // The same service can also be resolved from the request
            context.RequestServices.GetRequiredService<ICustomService>();

            await context.Response.WriteAsync(_customService.GetMessage());
        });
    }
}
