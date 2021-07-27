using BiExcellence.OpenBi.Server.License.Abstractions;
using HtmlAgilityPack;
using Ibssolution.biox.Repositoryserver;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomHtmlItemExample
{
    public class CustomHtmlItem : HtmlItem
    {
        private readonly ILicense _license;
        private readonly string _templateId;

        // Get ILicense from DI
        public CustomHtmlItem(HtmlNode node, ILicense license)
            : this(node)
        {
            _license = license;
        }

        // Called once when server starts with null node
        public CustomHtmlItem(HtmlNode node)
            : base(node)
        {
            // Read passed in attributes
            Attributes.TryGetValue("data-template", out _templateId);
        }

        // Called for GET requests when used in HTML
        protected override async Task<string> GetHtmlFromTagChildAsync(CmsDescription cms, CancellationToken cancellationToken)
        {
            // Get item template
            var template = await GetItemTemplateAsync(_templateId, cms);

            // Check if user is authentificated
            if (cms.HttpContext.User.Identity.IsAuthenticated)
            {
                template = template.Replace("%USERNAME%", cms.OpenBiRequest.User.Username);
                template = template.Replace("%USERNAME%", cms.HttpContext.User.Identity.Name); // same as above
            }

            template = template.Replace("%LICENSE_NAME%", _license.Name);

            // append the InnerHtml from the HTML
            template += HtmlNode.InnerHtml;

            return template;
        }

        // Called for form POST requests
        protected override async Task ProcessActionChildAsync(CmsDescription cms, string action, CancellationToken cancellationToken)
        {
            // Check the action
            if (action == "customaction")
            {
                // Get the form from the request
                var form = await cms.HttpContext.Request.ReadFormAsync();

                // Change response Content-Type
                cms.HttpContext.Response.ContentType = "text/html";

                // Check if user is authentificated
                if (cms.HttpContext.User.Identity.IsAuthenticated)
                {
                    await cms.HttpContext.Response.WriteAsync($"<h1>Username: {cms.OpenBiRequest.User.Username}</h1>");
                    await cms.HttpContext.Response.WriteAsync($"<h1>Username: {cms.HttpContext.User.Identity.Name}</h1>"); // same as above
                }

                await cms.HttpContext.Response.WriteAsync($"License Name: {_license.Name}");
            }
        }

        // Document possible attributes
        protected override void AddParametersToCollection(HtmlItemParameterCollection collection)
        {
            collection.Add(new HtmlItemParameter("data-template", HtmlItemParameterType.Template, "Custom Template"));
        }

        // Document possible replacements
        protected override HtmlItemReplacementParameterCollection getReplacementParameters()
        {
            var replacements = new HtmlItemReplacementParameterCollection();
            replacements.Add(new HtmlItemReplacementParameter("%USERNAME%", "Username"));
            replacements.Add(new HtmlItemReplacementParameter("%LICENSE_NAME%", "License Name"));
            return replacements;
        }

        // Unique HTML tag name which will instantiate a new instance of this class when found the HTML
        public override string GetTagName()
        {
            return "custom:htmlitem";
        }

        // Document HTML item
        public override string GetDescription()
        {
            return "My Custom HTML Item";
        }

    }
}