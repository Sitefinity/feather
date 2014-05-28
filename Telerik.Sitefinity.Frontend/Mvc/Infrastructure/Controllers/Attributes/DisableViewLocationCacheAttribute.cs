using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// Actions marked with this attribute will not use cache for their view locations.
    /// </summary>
    public class DisableViewLocationCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller == null)
                throw new InvalidOperationException(DisableViewLocationCacheAttribute.InvalidControllerMessage);

            var voidViewLocationCache = new VoidViewLocationCache();
            this.originalViewLocationCache = new IViewLocationCache[controller.ViewEngineCollection.Count];
            for (int i = 0; i < controller.ViewEngineCollection.Count; i++)
            {
                var vppEngine = controller.ViewEngineCollection[i] as VirtualPathProviderViewEngine;
                if (vppEngine != null)
                {
                    this.originalViewLocationCache[i] = vppEngine.ViewLocationCache;
                    vppEngine.ViewLocationCache = voidViewLocationCache;
                }
            }
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller == null)
                throw new InvalidOperationException(DisableViewLocationCacheAttribute.InvalidControllerMessage);

            for (int i = 0; i < controller.ViewEngineCollection.Count; i++)
            {
                var vppEngine = controller.ViewEngineCollection[i] as VirtualPathProviderViewEngine;
                if (vppEngine != null && i < originalViewLocationCache.Length)
                {
                    vppEngine.ViewLocationCache = originalViewLocationCache[i];
                }
            }
        }

        private IViewLocationCache[] originalViewLocationCache;
        private const string InvalidControllerMessage = "DisableViewLocation attribute can be set only on actions of controllers that inherit from System.Web.Mvc.Controller.";
    }
}
