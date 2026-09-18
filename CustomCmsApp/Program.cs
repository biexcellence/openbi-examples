using Ibssolution.biox.Repositoryserver;

namespace CustomCmsAppExample;

// A CMS app claims the first segment of the URL path, here everything below "/customcmsapp".
// The <openbi:cmsapp>, <openbi:cmsapplist> and <openbi:cmsappnavbar> items render the app of the
// current URL, the list of all installed apps and the navigation of the current app.
[OpenBiCmsApp(Id = "customcmsapp", Name = "My Custom CMS App")]
public sealed class CustomCmsApp : IOpenBiCmsApp
{
}
