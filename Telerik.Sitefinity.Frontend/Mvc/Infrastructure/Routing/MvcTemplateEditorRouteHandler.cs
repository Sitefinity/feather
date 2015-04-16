using System.Web;
using System.Web.Routing;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Extended version of the TemplateEditorRouteHandler that injects logic handling MVC layout files.
    /// </summary>
    internal class MvcTemplateEditorRouteHandler : TemplateEditorRouteHandler
    {
        /// <summary>
        /// Builds the handler.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="pageData">The page data.</param>
        /// <returns></returns>
        protected override IHttpHandler BuildHandler(RequestContext requestContext, IPageData pageData)
        {
            var handler = base.BuildHandler(requestContext, pageData);
            new PageInitializer().Initialize(handler);

            return handler;
        }
    }
}
