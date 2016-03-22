using System.Web.Mvc;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Action filter that terminates the request to a controller if the Frontend (Feather) Module is disabled or uninstalled.
    /// </summary>
    internal sealed class FrontendModuleFilter : ActionFilterAttribute, IActionFilter
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SystemManager.GetModule("Feather") == null && filterContext != null && filterContext.Controller != null && filterContext.Controller.GetType().FullName.StartsWith("Telerik.Sitefinity.Frontend"))
            {
                filterContext.Result = new EmptyResult();
            }
        }
    }
}