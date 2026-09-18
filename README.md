# open bi Examples

This repository contains examples for extending the [open bi server](https://openbi.info) with plugins.

See the [plugin documentation](https://openbi.info/documentation/Plugins) for the details of every extension point.

## Getting started

A plugin is a .NET class library which references the assemblies of the open bi server installation directly - the server assemblies are not published on NuGet.

1. Install the open bi server, by default into `C:\OPENBI`.
2. Build an example. A debug build copies the plugin assembly into `C:\OPENBI\HttpServer\plugins`, which is where the open bi server loads it from.
3. Restart the open bi server. Loaded plugins are logged on startup with their name, version and path.

Every example is self contained: its project file has the target framework, the references into the installation directory and the copy target of the debug build. If the open bi server is not installed in `C:\OPENBI`, change the paths in the project file of the example.

`ibssolution.bioxRepository.dll` and `BiExcellence.OpenBi.Server.License.Abstractions.dll` are always needed, every other reference depends on what the plugin uses. Because the open bi server is deployed with all its dependencies, every assembly it uses can be referenced the same way, for example `HtmlAgilityPack.dll` for an HTML item or `Microsoft.AspNetCore.Http.Abstractions.dll` for an `HttpContext`.

To debug a plugin, start the project with the Visual Studio profile of the example, which launches `ibssolution.bioxRepository.exe` from the installation directory, or use the launch configurations in [.vscode](.vscode).

## Examples

### [CustomCommandHandler](CustomCommandHandler)

Add own commands to the legacy command API with `ICommandApiHandler`.

Related blog post: https://openbi.info/blog/open-bi-server-custom-command-handler-plugin

### [CustomHttpHandler](CustomHttpHandler)

Add own HTTP endpoints, route groups and middleware with `IOpenBiStartup`.

Related blog post: https://openbi.info/blog/open-bi-server-custom-http-handler-plugin

### [CustomHtmlItem](CustomHtmlItem)

Add own CMS items with `HtmlItem`, including templates, placeholders and POST actions.

Related blog post: https://openbi.info/blog/open-bi-server-custom-htmlitem-plugin

### [CustomBatchJobHandler](CustomBatchJobHandler)

Add own job types with `BatchJobHandler`, including parameters, the job log and custom runtimes.

Related blog post: https://openbi.info/blog/open-bi-server-custom-batch-job-handler-plugin

### [CustomPlugin](CustomPlugin)

Register own services with `IOpenBiPlugin` and inject them into the other extension points.

### [CustomComponent](CustomComponent)

Add own Blazor components for the CMS content.

### [CustomCmsApp](CustomCmsApp)

Claim a URL path for an own CMS app with `IOpenBiCmsApp`.

### [CustomDatabase](CustomDatabase)

Create an own table in the open bi database and read and write it with `IEntityDatabaseFactory`.

### [CustomBackgroundService](CustomBackgroundService)

Run own background work while the open bi server is running with `IHostedService`.

## Requirements

The examples target the current open bi server release. The .NET version of a plugin has to match the one of the server, see [open bi server versions](https://openbi.info/documentation).

The examples on `main` target open bi server 4.5.x (.NET 10). For older server versions, check out the commit which updated the examples for that version.
