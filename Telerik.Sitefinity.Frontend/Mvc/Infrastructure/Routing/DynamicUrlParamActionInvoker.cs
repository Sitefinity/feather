using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Extends <see cref="Telerik.Sitefinity.Mvc.ControllerActionInvoker"/> by providing logic for custom routing.
    /// </summary>
    internal class DynamicUrlParamActionInvoker : Telerik.Sitefinity.Mvc.ControllerActionInvoker
    {
        /// <inheritdoc/>
        protected override void InitializeRouteParameters(MvcProxyBase proxyControl)
        {
            RouteHelper.SetUrlParametersResolved(false);

            var originalContext = proxyControl.Context.Request.RequestContext ?? proxyControl.Page.GetRequestContext();

            var paramsMapper = this.GetDefaultParamsMapper(proxyControl.Controller);
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
            IUrlParamsMapper result = null;

            result = result
                .SetLast(DetailActionParamsMapper.GetInferredDetailActionParamsMapper(controller))
                .SetLast(this.GetInferredTaxonFilterMapper(controller, "ListByTaxon"))
                .SetLast(this.GetInferredPagingMapper(controller, "Index"))
                .SetLast(new DefaultUrlParamsMapper(controller));

            return result;
        }

        private IUrlParamsMapper GetInferredTaxonFilterMapper(ControllerBase controller, string actionName)
        {
            var actionDescriptor = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);

            if (actionDescriptor == null || actionDescriptor.GetParameters().Length == 0)
                return null;

            IUrlParamsMapper result = null;
            if (actionDescriptor.GetParameters()[0].ParameterType == typeof(ITaxon))
            {
                var taxonParamName = actionDescriptor.GetParameters()[0].ParameterName;
                if (actionDescriptor.GetParameters()[1].ParameterType == typeof(int?))
                {
                    var pageParamName = actionDescriptor.GetParameters()[1].ParameterName;
                    result = new CustomActionParamsMapper(controller, () => "/{" + taxonParamName + ":category,tag}/{" + pageParamName + "}", actionName);
                }

                result = result.SetLast(new CustomActionParamsMapper(controller, () => "/{" + taxonParamName + ":category,tag}", actionName));
            }

            return result;
        }

        private IUrlParamsMapper GetInferredPagingMapper(ControllerBase controller, string actionName)
        {
            var actionDescriptor = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);

            if (actionDescriptor == null || actionDescriptor.GetParameters().Length == null || actionDescriptor.GetParameters()[0].ParameterType != typeof(int?))
                return null;

            return new CustomActionParamsMapper(controller, () => "/{" + actionDescriptor.GetParameters()[0].ParameterName + "}", actionName);
        }

        /// <summary>
        /// The controller name key
        /// </summary>
        public const string ControllerNameKey = "controller";
    }
}
