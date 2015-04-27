using System.Web;
using System.Web.Routing;
using System.Web.UI;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Represents an extended route handler for Sitefinity's page editor that injects MVC specific logic.
    /// </summary>
    public class MvcPageEditorRouteHandler : PageEditorRouteHandler
    {
        /// <summary>
        /// Builds a handler for the specified request and page data.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="pageData">The page data.</param>
        /// <returns>The handelr.</returns>
        protected override IHttpHandler BuildHandler(RequestContext requestContext, IPageData pageData)
        {
            var handler = base.BuildHandler(requestContext, pageData);
            new PageInitializer().Initialize(handler);

            var page = handler as Page;
            if (page != null)
            {
                page.Init += this.MvcPageEditorRouteHandlerInit;
            }

            return handler;
        }

        private void MvcPageEditorRouteHandlerInit(object sender, System.EventArgs e)
        {
            var page = (Page)sender;
            
            //// TODO: Uncomment this when css is ready
            //// page.Header.Controls.Add(new LiteralControl(this.GetInlineStyle()));
        }

        private string GetInlineStyle()
        {
            //// TODO: Change icon
            var iconUrl = "client-components/temp_mvc_logo.png";

            var fullIconUrl = RouteHelper.ResolveUrl(string.Format("~/{0}/{1}", FrontendManager.VirtualPathBuilder.GetVirtualPath(this.GetType().Assembly), iconUrl), UrlResolveOptions.Rooted);
            var inlineCss = @".sfMvcIcn { background: transparent url(" + fullIconUrl + ") no-repeat 0 !important; }";
            return string.Format(@"<style type=""text/css"">{0}</style>", inlineCss);
        }
    }
}
