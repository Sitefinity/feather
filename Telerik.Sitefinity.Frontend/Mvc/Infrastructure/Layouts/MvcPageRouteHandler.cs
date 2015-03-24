using System;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    internal class MvcPageRouteHandler : Telerik.Sitefinity.Web.PageRouteHandler
    {
        protected override IHttpHandler BuildHttpHandler(RequestContext requestContext)
        {
            var handler = base.BuildHttpHandler(requestContext);
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
