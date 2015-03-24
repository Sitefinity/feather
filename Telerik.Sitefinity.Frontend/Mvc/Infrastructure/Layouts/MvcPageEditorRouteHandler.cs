using System;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    internal class MvcPageEditorRouteHandler : PageEditorRouteHandler
    {
        protected override IHttpHandler BuildHandler(RequestContext requestContext, IPageData pageData)
        {
            var handler = base.BuildHandler(requestContext, pageData);
            var page = handler as Page;
            if (page != null)
            {
                page.PreInit += this.PreInitHandler;
            }

            return handler;
        }

        private void PreInitHandler(object sender, EventArgs e)
        {
            var page = (Page)sender;
            if (LayoutMvcPageResolver.IsLayoutPath(page.MasterPageFile))
            {
                this.DynamicMasterPage(page);
            }
        }

        private void DynamicMasterPage(Page page)
        {
            page.Request.RequestContext.HttpContext.Items.Remove("JsRegister");
            new MvcMasterPage().ApplyToPage(page);
        }
    }
}
