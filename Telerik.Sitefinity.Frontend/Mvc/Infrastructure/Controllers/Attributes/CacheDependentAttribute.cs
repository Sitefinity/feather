using System;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// This class represents an action filter that makes sure if a ViewResult is executed the View file has its cache dependency added to the response.
    /// </summary>
    public class CacheDependentAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework after the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            if (SystemManager.CurrentHttpContext != null && SystemManager.CurrentHttpContext.Response != null)
            {
                var viewResult = filterContext.Result as ViewResultBase;
                if (viewResult != null)
                {
                    var builtView = viewResult.View as BuildManagerCompiledView;
                    if (builtView != null)
                    {
                        var cacheDependency = this.GetCacheDependency(builtView.ViewPath);
                        if (cacheDependency != null)
                        {
                            SystemManager.CurrentHttpContext.Response.AddCacheDependency(cacheDependency);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cache dependency for the given virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        protected virtual CacheDependency GetCacheDependency(string virtualPath)
        {
            if (HostingEnvironment.VirtualPathProvider != null)
            {
                return HostingEnvironment.VirtualPathProvider.GetCacheDependency(virtualPath, null, DateTime.UtcNow);
            }

            return null;
        }
    }
}
