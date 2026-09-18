using BiExcellence.OpenBi.Server.Plugin.Abstractions;
using Ibssolution.biox.Repositoryserver;
using Microsoft.Extensions.DependencyInjection;

namespace CustomPluginExample;

// An assembly which contains an IOpenBiPlugin registers its own services, and from then on the open bi
// server only uses the registered ones - the extension points of this assembly which are resolved from
// the service collection (IOpenBiStartup, ICommandApiHandler, IHostedService, ...) have to be registered
// here as well. Without an IOpenBiPlugin, all types of the assembly are instantiated automatically.
public sealed class CustomPlugin : IOpenBiPlugin
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        // Own services, with the lifetime the plugin needs
        serviceCollection.AddSingleton<ICustomService, CustomService>();

        // The extension points of this assembly
        serviceCollection.AddSingleton<IOpenBiStartup, CustomStartup>();
    }
}

public interface ICustomService
{
    string GetMessage();
}

internal sealed class CustomService : ICustomService
{
    public string GetMessage() => "Hello from ICustomService";
}
