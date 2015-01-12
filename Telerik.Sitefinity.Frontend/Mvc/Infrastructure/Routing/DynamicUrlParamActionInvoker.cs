using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Pages.Configuration;
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
        protected override bool ShouldProcessRequest(MvcProxyBase proxyControl)
        {
            var shouldProcess = base.ShouldProcessRequest(proxyControl);

            var configManager = ConfigManager.GetManager();
            var toolboxesConfig = configManager.GetSection<ToolboxesConfig>();
            shouldProcess &= toolboxesConfig != null;

            return shouldProcess;
        }

        /// <inheritdoc/>
        protected override void InitializeRouteParameters(MvcProxyBase proxyControl)
        {
            var originalContext = proxyControl.Context.Request.RequestContext ?? proxyControl.Page.GetRequestContext();

            this.SetControllerRouteParam(proxyControl);

            var paramsMapper = this.GetDefaultParamsMapper(proxyControl.Controller);
            if (paramsMapper != null)
            {
                var originalParams = MvcRequestContextBuilder.GetRouteParams(originalContext);
                var requestContext = proxyControl.RequestContext;

                paramsMapper.ResolveUrlParams(originalParams, requestContext);
                proxyControl.Controller.TempData.Add("IsInPureMode", proxyControl.IsInPureMode);

                if (!proxyControl.ContentTypeName.IsNullOrEmpty())
                    proxyControl.Controller.RouteData.Values.Add("contentTypeName", proxyControl.ContentTypeName);
            }
            else
            {
                proxyControl.RequestContext.RouteData.Values.Remove(DynamicUrlParamActionInvoker.ControllerNameKey);
                base.InitializeRouteParameters(proxyControl);
            }
        }

        /// <summary>
        /// Logs exceptions thrown by the invocation of <see cref="ControllerActionInvoker"/>
        /// </summary>
        /// <param name="proxyControl">The proxy control.</param>
        protected override void ExecuteController(MvcProxyBase proxyControl)
        {
            try
            {
                base.ExecuteController(proxyControl);
            }
            catch (Exception ex)
            {
                if (Exceptions.HandleException(ex, ExceptionPolicyName.IgnoreExceptions))
                    throw;

                proxyControl.Context.Response.Clear();

                if (this.ShouldDisplayErrors())
                    proxyControl.Context.Response.Write(Res.Get<InfrastructureResources>().ErrorExecutingController);
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

        /// <summary>
        /// Determines whether errors should be displayed.
        /// </summary>
        /// <returns>True if errors should be displayed, False to fail silently.</returns>
        protected virtual bool ShouldDisplayErrors()
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~/Web.config");
            var customErrors = configuration.GetSection("system.web/customErrors") as CustomErrorsSection;

            if (customErrors == null)
                return true;

            return customErrors.Mode == CustomErrorsMode.Off || (customErrors.Mode == CustomErrorsMode.RemoteOnly && HttpContext.Current.Request.IsLocal);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private IEnumerable<string> GetProviderNames(ControllerBase controller, Type contentType)
        {
            var providerNameProperty = controller.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.Name == "ProviderName" && p.PropertyType == typeof(string));

            if (providerNameProperty != null)
            {
                return new string[1] { providerNameProperty.GetValue(controller, null) as string };
            }
            else
            {
                IManager manager;

                try
                {
                    ManagerBase.TryGetMappedManager(contentType, string.Empty, out manager);
                }
                catch (Exception ex)
                {
                    Log.Write(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Exception occurred in the routing functionality, details: {0}", ex));
                    manager = null;
                }

                if (manager != null)
                {
                    return manager.Providers.Select(p => p.Name);
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
                        var dynamicContentType = controller.GetDynamicContentType();
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

            return new CustomActionParamsMapper(controller, () => "/{" + actionDescriptor.GetParameters()[0].ParameterName + ":int}", actionName);
        }

        private void SetControllerRouteParam(MvcProxyBase proxyControl)
        {
            var requestContext = proxyControl.RequestContext;

            string controllerName;
            var widgetProxy = proxyControl as MvcWidgetProxy;
            if (widgetProxy != null && !string.IsNullOrEmpty(widgetProxy.WidgetName))
            {
                controllerName = widgetProxy.WidgetName;
            }
            else
            {
                controllerName = SitefinityViewEngine.GetControllerName(proxyControl.Controller);
            }

            requestContext.RouteData.Values[DynamicUrlParamActionInvoker.ControllerNameKey] = controllerName;
        }

        /// <summary>
        /// The controller name key
        /// </summary>
        public const string ControllerNameKey = "controller";
    }
}
