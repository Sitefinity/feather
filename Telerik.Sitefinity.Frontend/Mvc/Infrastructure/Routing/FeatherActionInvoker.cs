using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
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
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Extends <see cref="Telerik.Sitefinity.Mvc.ControllerActionInvoker"/> by providing logic for custom routing.
    /// </summary>
    public class FeatherActionInvoker : Telerik.Sitefinity.Mvc.ControllerActionInvoker
    {
        /// <summary>
        /// Gets the default parameters mapper.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>The default parameters mapper for the controller.</returns>
        internal IUrlParamsMapper GetDefaultParamsMapper(ControllerBase controller)
        {
            IUrlParamsMapper result = null;
            result = result
                .SetLast(this.GetInferredDetailActionParamsMapper(controller))
                .SetLast(this.GetInferredTaxonFilterMapper(controller, "ListByTaxon"))
                .SetLast(this.GetInferredClassificationFilterMapper(controller, "ListByTaxon"))
                .SetLast(this.GetInferredPagingMapper(controller, "Index"));

            // If no other mappers are added we skip the default one.
            if (result != null)
                result.SetLast(new DefaultUrlParamsMapper(controller));

            return result;
        }

        /// <inheritdoc/>
        protected override bool ShouldProcessRequest(MvcProxyBase proxyControl)
        {
            var shouldProcess = base.ShouldProcessRequest(proxyControl);

            var toolboxesConfig = Config.Get<ToolboxesConfig>();
            shouldProcess &= toolboxesConfig != null;

            return shouldProcess;
        }

        /// <inheritdoc/>
        protected override void InitializeRouteParameters(MvcProxyBase proxyControl)
        {
            var originalContext = this.Context.Request.RequestContext ?? proxyControl.Page.GetRequestContext();

            this.SetControllerRouteParam(proxyControl);

            var controller = proxyControl.GetController();

            if (!FrontendManager.AttributeRouting.HasAttributeRouting(controller.RouteData))
            {
                var paramsMapper = this.GetDefaultParamsMapper(controller);
                if (paramsMapper != null)
                {
                    var originalParams = MvcRequestContextBuilder.GetRouteParams(originalContext);
                    var requestContext = proxyControl.RequestContext;

                    paramsMapper.ResolveUrlParams(originalParams, requestContext);

                    if (!proxyControl.ContentTypeName.IsNullOrEmpty())
                        controller.RouteData.Values.Add("contentTypeName", proxyControl.ContentTypeName);
                }
                else
                {
                    proxyControl.RequestContext.RouteData.Values.Remove(FeatherActionInvoker.ControllerNameKey);
                    base.InitializeRouteParameters(proxyControl);
                }
            }
            else
            {
                if (FrontendManager.AttributeRouting.UpdateRouteData(this.Context, controller.RouteData))
                {
                    //// Attribute routing was successful.
                    RouteHelper.SetUrlParametersResolved();
                }
            }

            controller.TempData.Add("IsInPureMode", proxyControl.IsInPureMode);
        }

        /// <summary>
        /// Logs exceptions thrown by the invocation of <see cref="ControllerActionInvoker"/>
        /// </summary>
        /// <param name="proxyControl">The proxy control.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void ExecuteController(MvcProxyBase proxyControl)
        {
            var controller = proxyControl.GetController();
            if (proxyControl.IsIndexingMode() && controller.GetIndexRenderMode() == IndexRenderModes.NoOutput)
                return;

            try
            {
                this.TryLoadTempData(controller);
                base.ExecuteController(proxyControl);
            }
            catch (Exception ex)
            {
                this.HandleControllerException(ex);
            }
            finally
            {
                this.TrySaveTempData(controller);
            }
        }

        /// <summary>
        /// Handles the exception that occurred when executing the controller.
        /// </summary>
        /// <param name="err">The exception.</param>
        protected virtual void HandleControllerException(Exception err)
        {
            if (!(err is ThreadAbortException))
                if (Exceptions.HandleException(err, ExceptionPolicyName.IgnoreExceptions))
                    throw err;

            this.Context.Response.Clear();

            if (this.ShouldDisplayErrors())
                this.Context.Response.Write(Res.Get<InfrastructureResources>().ErrorExecutingController);
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

        private IUrlParamsMapper GetInferredClassificationFilterMapper(ControllerBase controller, string actionName)
        {
            var actionDescriptor = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);

            if (actionDescriptor == null || actionDescriptor.GetParameters().Length == 0)
                return null;

            return new TaxonomyUrlParamsMapper(controller, new TaxonUrlMapper(new TaxonUrlEvaluatorAdapter()));
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
                controllerName = SitefinityViewEngine.GetControllerName(proxyControl.GetController());
            }

            requestContext.RouteData.Values[FeatherActionInvoker.ControllerNameKey] = controllerName;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void TrySaveTempData(Controller controller)
        {
            try
            {
                if (controller != null && controller.ControllerContext != null && controller.Session != null && !controller.ControllerContext.IsChildAction)
                {
                    controller.TempData.Save(controller.ControllerContext, controller.TempDataProvider);
                }
            }
            catch (Exception ex)
            {
                var exceptionMessage = string.Format(
                        "Failed to Save TempData. Class: {0}, Method: {1}, Exception {2}, Stack Trace {3}",
                        this.GetType().Name,
                        System.Reflection.MethodInfo.GetCurrentMethod().Name,
                        ex.Message,
                        ex.StackTrace);

                Log.Write(exceptionMessage, ConfigurationPolicy.ErrorLog);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void TryLoadTempData(Controller controller)
        {
            try
            {
                if (controller != null && controller.ControllerContext != null && controller.Session != null && !controller.ControllerContext.IsChildAction)
                {
                    // Saving the current temp data.
                    var oldTempData = new TempDataDictionary();
                    foreach (var kv in controller.TempData)
                    {
                        oldTempData.Add(kv.Key, kv.Value);
                    }

                    // Loading the temp data removes all current temp data.
                    controller.TempData.Load(controller.ControllerContext, controller.TempDataProvider);

                    // Restoring the current temp data.
                    foreach (var kv in oldTempData)
                    {
                        controller.TempData[kv.Key] = kv.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                var exceptionMessage = string.Format(
                    "Failed to Load TempData. Class: {0}, Method: {1}, Exception {2}, Stack Trace {3}",
                    this.GetType().Name,
                    System.Reflection.MethodInfo.GetCurrentMethod().Name,
                    ex.Message,
                    ex.StackTrace);

                Log.Write(exceptionMessage, ConfigurationPolicy.ErrorLog);
            }
        }

        /// <summary>
        /// The controller name key
        /// </summary>
        public const string ControllerNameKey = "controller";
    }
}
