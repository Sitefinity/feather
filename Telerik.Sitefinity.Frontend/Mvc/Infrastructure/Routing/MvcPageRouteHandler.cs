using System.Web;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Extended route handler for Sitefinity pages that injects MVC specific logic.
    /// </summary>
    public class MvcPageRouteHandler : Telerik.Sitefinity.Web.PageRouteHandler
    {
        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        /// <returns>An object that processes the request.</returns>
        protected override IHttpHandler BuildHttpHandler(RequestContext requestContext)
        {
            var handler = base.BuildHttpHandler(requestContext);
            new PageInitializer().Initialize(handler);

            return handler;
        }
    }
}
