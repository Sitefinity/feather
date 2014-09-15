using System;
using System.Web.Mvc;
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
            IUrlParamsMapper paramsMapper;
            if (urlController != null)
            {
                paramsMapper = urlController.UrlParamsMapper;
            }
            else
            {
                paramsMapper = this.GetDefaultParamsMapper(proxyControl.Controller);
            }

            if (paramsMapper != null)
            {
                var requestContext = proxyControl.RequestContext;
                var originalParams = MvcRequestContextBuilder.GetRouteParams(originalContext);
                var controllerName = SitefinityViewEngine.GetControllerName(proxyControl.Controller);
                requestContext.RouteData.Values[DynamicUrlParamActionInvoker.ControllerNameKey] = controllerName;

                paramsMapper.ResolveUrlParams(originalParams, requestContext);
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
        /// Gets the default parameters mapper.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>The default parameters mapper for the controller.</returns>
        protected virtual IUrlParamsMapper GetDefaultParamsMapper(ControllerBase controller)
        {
            var defaultParamsMapper = new DefaultUrlParamsMapper(controller);
            IUrlParamsMapper result = defaultParamsMapper;

            IUrlParamsMapper detailsMapper = DetailActionParamsMapper.GetInferredDetailActionParamsMapper(controller);
            if (detailsMapper != null)
            {
                result = detailsMapper;

                while (detailsMapper.Next != null)
                    detailsMapper = detailsMapper.Next;

                detailsMapper.SetNext(defaultParamsMapper);
            }

            return result;
        }

        /// <summary>
        /// The controller name key
        /// </summary>
        public const string ControllerNameKey = "controller";
    }
}
