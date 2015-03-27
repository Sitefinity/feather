using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

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

            var page = handler as Page;
            if (page != null)
            {
                this.Initialize(page);
            }
        }

        private void Initialize(Page page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (page != null)
            {
                page.PreInit += this.PreInitHandler;
                page.Init += this.InitHandler;
            }
        }

        private void InitHandler(object sender, EventArgs e)
        {
            var page = (Page)sender;

            if (page.Header != null)
            {
                var scriptRenderer = new StyleSheetRenderer();
                page.Header.Controls.Add(scriptRenderer);
            }
        }

        private void PreInitHandler(object sender, EventArgs e)
        {
            var page = (Page)sender;
            if (LayoutMvcPageResolver.IsLayoutPath(page.MasterPageFile))
            {
                page.Request.RequestContext.HttpContext.Items.Remove("JsRegister");
                new MvcMasterPage().ApplyToPage(page);
            }
        }
    }
}
