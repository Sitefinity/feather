using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class call the default Sitefinity logic for mapping URL parameters to route data.
    /// </summary>
    internal class DefaultUrlParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUrlParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public DefaultUrlParamsMapper(ControllerBase controller)
            : base(controller)
        {
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext)
        {
            var selfRouting = this.Controller as IRouteMapper;
            if (urlParams != null && selfRouting != null && selfRouting.TryMapRouteParameters(urlParams, requestContext))
            {
                RouteHelper.SetUrlParametersResolved(true);
                return true;
            }

            var controllerName = requestContext.RouteData.Values[FeatherActionInvoker.ControllerNameKey] as string;
            string actionName = null;
            if (requestContext.RouteData.Values.ContainsKey("action"))
            {
                actionName = requestContext.RouteData.Values["action"] as string;
                requestContext.RouteData.Values.Remove("action");
            }

            try
            {
                requestContext.RouteData.Values.Remove(FeatherActionInvoker.ControllerNameKey);
                MvcRequestContextBuilder.SetRouteParameters(urlParams, requestContext, this.Controller as Controller, controllerName);
            }
            finally
            {
                if (actionName != null)
                {
                    requestContext.RouteData.Values["action"] = actionName;
                }
            }

            return true;
        }
    }
}
