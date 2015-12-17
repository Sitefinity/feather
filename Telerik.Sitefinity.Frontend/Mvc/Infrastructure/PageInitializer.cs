using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure
{
    /// <summary>
    /// Instances of this are used to manipulated a page right after it is created.
    /// </summary>
    internal class PageInitializer
    {
        /// <summary>
        /// Initializes the specified handler as a page. This method is called right after the handler is created and can be used to add controls to the Page.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <exception cref="System.ArgumentNullException">handler</exception>
        public void Initialize(IHttpHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            Page page = handler.GetPageHandler();

            if (page != null)
            {
                this.Initialize(page);
            }
        }

        /// <summary>
        /// Gets the inline style.
        /// </summary>
        /// <returns></returns>
        public string GetInlineStyle()
        {
            var loadingUrl = "assets/dist/css/loading.min.css";

            var fullLoadingUrl = RouteHelper.ResolveUrl(string.Format("~/{0}{1}", FrontendManager.VirtualPathBuilder.GetVirtualPath(this.GetType().Assembly), loadingUrl), UrlResolveOptions.Rooted);
            var cssValue = @".sfMvcIcn { position: relative; } .sfMvcIcn:after { content: ""MVC""; display: block; position: absolute; padding: 2px 3px; left: 26px; bottom: 4px; background: #105CB6; color: #fff; font-size: 8px;} a.sfAddContentLnk.sfMvcIcn:after{ bottom: 24px; left: calc(50% - 2px); }";
            var inlineCss = string.Format(@"<style type=""text/css"">{0}</style><link rel=""stylesheet"" type=""text/css"" href=""{1}"">", cssValue, fullLoadingUrl);

            return inlineCss;
        }

        private void Initialize(Page page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (page != null)
            {
                page.PreInit += this.PreInitHandler;
            }
        }

        private void PreInitHandler(object sender, EventArgs e)
        {
            var page = ((IHttpHandler)sender).GetPageHandler();

            if (LayoutMvcPageResolver.IsLayoutPath(page.MasterPageFile))
            {
                page.Request.RequestContext.HttpContext.Items.Remove("JsRegister");
                new MvcMasterPage().ApplyToPage(page);
            }
        }
    }
}
