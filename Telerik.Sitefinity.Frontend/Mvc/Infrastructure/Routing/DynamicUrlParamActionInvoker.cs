using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Extends <see cref="Telerik.Sitefinity.Mvc.ControllerActionInvoker"/> by providing logic for custom routing.
    /// </summary>
    public class DynamicUrlParamActionInvoker : Telerik.Sitefinity.Mvc.ControllerActionInvoker
    {
        /// <inheritdoc/>
        protected override void InitializeRouteParameters(MvcProxyBase proxyControl)
        {
            RouteHelper.SetUrlParametersResolved(false);

            var originalContext = proxyControl.Context.Request.RequestContext ?? proxyControl.Page.GetRequestContext();

            var urlController = proxyControl.Controller as IUrlMappingController;
            if (urlController != null)
            {
                var requestContext = proxyControl.RequestContext;
                var originalParams = MvcRequestContextBuilder.GetRouteParams(originalContext);
                var controllerName = SitefinityViewEngine.GetControllerName(proxyControl.Controller);
                requestContext.RouteData.Values[DynamicUrlParamActionInvoker.ControllerNameKey] = controllerName;
                urlController.UrlParamsMapper.ResolveUrlParams(originalParams, requestContext);
                proxyControl.Controller.TempData.Add("IsInPureMode", proxyControl.IsInPureMode);

                if (!proxyControl.ContentTypeName.IsNullOrEmpty())
                    proxyControl.Controller.RouteData.Values.Add("contentTypeName", proxyControl.ContentTypeName);
            }
            else
            {
                base.InitializeRouteParameters(proxyControl);
            }
        }

        /// <summary>
        /// The controller name key
        /// </summary>
        public const string ControllerNameKey = "controller";
    }
}
