using BiExcellence.OpenBi.Server.License.Abstractions;
using Ibssolution.biox.Repositoryserver;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomHttpHandlerExample
{
    public class CustomHttpHandler : IAsyncHttpHandler
    {
        private readonly ILicense _license;

        // Get ILicense from DI
        public CustomHttpHandler(ILicense license)
        {
            _license = license;
        }

        public async Task<bool> ProcessRequest(HttpContext context, ICollection<NameValue> parameters)
        {
            // Check the request path
            if (context.Request.Path.StartsWithSegments("/mycustomhttphandler"))
            {
                context.Response.ContentType = "text/html";

                // Check if user is authentificated
                if (context.User.Identity.IsAuthenticated)
                {
                    await context.Response.WriteAsync($"<h1>Username: {context.User.Identity.Name}</h1>");
                }

                await context.Response.WriteAsync($"License Name: {_license.Name}");

                // Mark request as handled
                return true;
            }
            // Request not handled
            return false;
        }
    }
}