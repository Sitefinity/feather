using System.Web;
using System.Web.Routing;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Extended route handler for Sitefinity versioning that injects MVC specific logic.
    /// </summary>
    public class MvcVersioningRouteHandler : VersioningRouteHandler
    {
        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        /// <returns>An object that processes the request.</returns>
        public override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var handler = base.GetHttpHandler(requestContext);
            new PageInitializer().Initialize(handler);

            return handler;
        }
    }
}
