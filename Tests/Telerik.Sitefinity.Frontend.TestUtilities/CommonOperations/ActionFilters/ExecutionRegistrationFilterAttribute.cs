using System;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.ActionFilters
{
    /// <summary>
    /// This class represents filter attribute which registers the execution of every action.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1018:MarkAttributesWithAttributeUsage")]
    public class ExecutionRegistrationFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework after the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            
            var actionInfo = new ActionInfo();
            actionInfo.Name = filterContext.RouteData.Values["action"].ToString();
            actionInfo.ActionRouteData = filterContext.RouteData;
            actionInfo.Result = filterContext.Result;
            actionInfo.CurrentHttpContext = filterContext.HttpContext;
            ActionExecutionRegister.ExecutedActionInfos.Add(actionInfo);
        }
    }
}
