using BiExcellence.OpenBi.Server.License.Abstractions;
using HtmlAgilityPack;
using Ibssolution.biox.Repositoryserver;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CustomHtmlItemExample;

// The tag name which instantiates this class when it is found in the CMS content
[HtmlItemTagName("custom:htmlitem")]
// Description shown in the Configurator
[Description("My Custom HTML Item")]
public sealed class CustomHtmlItem : HtmlItem
{
    private readonly ILicense _license;
    private readonly ILogger<CustomHtmlItem>? _logger;
    private readonly string? _templateId;

    // Used when the item renders, services are injected through the constructor
    public CustomHtmlItem(HtmlNode tag, ILicense license, ILogger<CustomHtmlItem> logger)
        : this(tag)
    {
        _license = license;
        _logger = logger;
    }

    // Used when the open bi server only reads the parameters of the item, the tag can be null
    public CustomHtmlItem(HtmlNode tag)
        : base(tag)
    {
        _license = null!;

        // Read the attributes which were set on the tag
        Attributes.TryGetValue("data-template", out _templateId);
    }

    // Called for GET requests when the tag is used in the CMS content
    protected override async Task<string?> GetHtmlFromTagChildAsync(CmsDescription cms, CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Rendering custom:htmlitem");

        // Load the HTML item template of the data-template attribute
        var template = "";
        if (_templateId is not null)
        {
            template = await GetItemTemplateAsync(_templateId, cms) ?? "";
        }

        // Check whether the user is authenticated
        if (cms.Session.User is not null)
        {
            template = template.Replace("%USERNAME%", cms.Session.User.Username);
        }

        template = template.Replace("%LICENSE_NAME%", _license.Name);

        // Append the inner HTML of the tag
        template += HtmlNode.InnerHtml;

        return template;
    }

    // Called for POST requests, see the cms_item and action fields in the CMS documentation
    protected override async Task ProcessActionChildAsync(CmsDescription cms, string action, CancellationToken cancellationToken)
    {
        if (action != "customaction") return;

        // Read the submitted form
        var form = await cms.HttpContext.Request.ReadFormAsync(cancellationToken);

        cms.HttpContext.Response.ContentType = "text/html";

        if (cms.Session.User is not null)
        {
            await cms.HttpContext.Response.WriteAsync($"<h1>Username: {cms.Session.User.Username}</h1>", cancellationToken);
        }

        await cms.HttpContext.Response.WriteAsync($"Value: {form["myfield"]}", cancellationToken);
    }

    // Additional CSS classes, stylesheets and scripts for the page
    public override List<string> GetBodyClasses()
    {
        var classes = base.GetBodyClasses();
        classes.Add("customhtmlitem");
        return classes;
    }

    // Document the attributes of the item for the Configurator
    protected override void AddParametersToCollection(HtmlItemParameterCollection collection)
    {
        collection.Add(new HtmlItemParameter("data-template", HtmlItemParameterType.Template, "Custom Template"));
    }

    // Document the placeholders which the templates of the item support
    protected override HtmlItemReplacementParameterCollection getReplacementParameters()
    {
        return [
            new HtmlItemReplacementParameter("data-template", "%USERNAME%", "Username"),
            new HtmlItemReplacementParameter("data-template", "%LICENSE_NAME%", "License Name")
        ];
    }
}
