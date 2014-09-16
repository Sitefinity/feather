using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.DynamicModules.Builder.Model;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
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
                .SetLast(this.GetInferredDetailActionParamsMapper(controller))
                .SetLast(this.GetInferredTaxonFilterMapper(controller, "ListByTaxon"))
                .SetLast(this.GetInferredPagingMapper(controller, "Index"));

            // If no other mappers are added we skip the default one.
            if (result != null)
                result.SetLast(new DefaultUrlParamsMapper(controller));

            return result;
        }

        private IEnumerable<string> GetProviderNames(ControllerBase controller, Type contentType)
        {
            var providerNameProperty = controller.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.Name == "ProviderName" && p.PropertyType == typeof(string));

            if (providerNameProperty != null)
            {
                return new string[1] { providerNameProperty.GetValue(controller, null) as string };
            }
            else
            {
                var mappedManager = ManagerBase.GetMappedManager(contentType);
                if (mappedManager != null)
                {
                    return mappedManager.Providers.Select(p => p.Name);
                }
                else
                {
                    return new string[0];
                }
            }
        }

        private IUrlParamsMapper GetInferredDetailActionParamsMapper(ControllerBase controller)
        {
            var controllerType = controller.GetType();
            IUrlParamsMapper result = null;

            var detailsAction = new ReflectedControllerDescriptor(controllerType).FindAction(controller.ControllerContext, DetailActionParamsMapper.DefaultActionName);
            if (detailsAction != null)
            {
                var contentParam = detailsAction.GetParameters().FirstOrDefault();
                if (contentParam != null && contentParam.ParameterType.ImplementsInterface(typeof(IDataItem)))
                {
                    Type contentType;
                    if (typeof(DynamicContent) == contentParam.ParameterType)
                    {
                        var dynamicContentType = this.GetDynamicContentType(controllerType);
                        contentType = dynamicContentType != null ? TypeResolutionService.ResolveType(dynamicContentType.GetFullTypeName(), throwOnError: false) : null;
                    }
                    else
                    {
                        contentType = contentParam.ParameterType;
                    }

                    if (contentType != null)
                    {
                        var providerNames = this.GetProviderNames(controller, contentType);
                        foreach (var provider in providerNames)
                        {
                            var providerName = provider;
                            result = result.SetLast(new DetailActionParamsMapper(controller, contentType, () => providerName));
                        }
                    }
                }
            }

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

            if (actionDescriptor == null || actionDescriptor.GetParameters().Length == 0 || actionDescriptor.GetParameters()[0].ParameterType != typeof(int?))
                return null;

            return new CustomActionParamsMapper(controller, () => "/{" + actionDescriptor.GetParameters()[0].ParameterName + "}", actionName);
        }

        private DynamicModuleType GetDynamicContentType(Type controllerType)
        {
            var moduleProvider = ModuleBuilderManager.GetManager().Provider;
            var controllerName = FrontendManager.ControllerFactory.GetControllerName(controllerType);
            var dynamicContentType = moduleProvider.GetDynamicModules().Where(m => m.Status == DynamicModuleStatus.Active)
                .Join(moduleProvider.GetDynamicModuleTypes().Where(t => t.TypeName == controllerName), m => m.Id, t => t.ParentModuleId, (m, t) => t)
                .FirstOrDefault();
            return dynamicContentType;
        }

        /// <summary>
        /// The controller name key
        /// </summary>
        public const string ControllerNameKey = "controller";
    }
}
