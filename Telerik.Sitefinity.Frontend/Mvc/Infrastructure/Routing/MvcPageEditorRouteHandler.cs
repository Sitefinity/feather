using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;

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

            var page = handler.GetPageHandler();
            if (page != null)
            {
                page.Init += this.MvcPageEditorRouteHandlerInit;
            }

            return handler;
        }

        /// <inheritDoc/>
        protected override void ApplyLayoutsAndControls(Page page, RequestContext requestContext)
        {
            base.ApplyLayoutsAndControls(page, requestContext);

            var context = Telerik.Sitefinity.Services.SystemManager.CurrentHttpContext.Items;
            var pageData = (Telerik.Sitefinity.Modules.Pages.PageDraftProxy)context["StaticPageDraft"];

            var field = typeof(Telerik.Sitefinity.Modules.Pages.DraftProxyBase).GetField("editor", BindingFlags.NonPublic | BindingFlags.Instance);
            var zoneEditor = field.GetValue(pageData) as ZoneEditor;

            zoneEditor.ControlAdd += this.ZoneEditor_ControlAdd;
        }

        private void ZoneEditor_ControlAdd(object sender, ZoneEditorEventArgs e)
        {
            if (e.ControlData.ObjectType == typeof(MvcControllerProxy).FullName)
            {
                var dockZone = (System.Web.UI.WebControls.WebControl)e.ControlContainer.NamingContainer;
                if (dockZone != null && dockZone.Attributes["perm_rollback"] == null)
                    dockZone.Attributes["perm_rollback"] = "False";
            }
        }

        private void MvcPageEditorRouteHandlerInit(object sender, System.EventArgs e)
        {
            var page = (Page)sender;

            page.Header.Controls.Add(new LiteralControl(new PageInitializer().GetInlineStyle()));
        }
    }
}
