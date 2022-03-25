using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Personalization;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing.Date;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Taxonomies;
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
                .SetLast(this.GetInferredSuccessorsActionParamsMapper(controller))
                .SetLast(this.GetInferredTaxonFilterQueryParamsMapper(controller, "ListByTaxon"))
                .SetLast(this.GetInferredTaxonFilterMapper(controller, "ListByTaxon"))
                .SetLast(this.GetInferredClassificationFilterMapper(controller, "ListByTaxon"))
                .SetLast(this.GetInferredDateFilterMapper(controller, "ListByDate"))
                .SetLast(this.GetInferredPagingMapper(controller, "Index"));

            // If no other mappers are added we skip the default one.
            if (result != null)
                result.SetLast(new DefaultUrlParamsMapper(controller));

            return result;
        }

        /// <summary>
        /// Gets the prefix parameters mapper.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        internal IUrlParamsMapper GetPrefixParamsMapper(ControllerBase controller)
        {
            IUrlParamsMapper result = null;
            result = result
                .SetLast(this.GetInferredSuccessorsActionParamsMapper(controller))
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

                    var modelProperty = controller.GetType().GetProperty("Model");
                    if (modelProperty != null)
                    {
                        var model = modelProperty.GetValue(controller, null);
                        var modelUrlKeyProperty = model == null ? null : model.GetType().GetProperty("UrlKeyPrefix");
                        var modelUrlKeyPrefix = modelUrlKeyProperty == null ? null : (string)modelUrlKeyProperty.GetValue(model, null);
                        var expectedUrlKeyPrefix = string.IsNullOrEmpty(modelUrlKeyPrefix) ? null : "!" + modelUrlKeyPrefix;
                        var currentUrlKeyPrefix = originalParams == null ? null : originalParams.FirstOrDefault(p => p.StartsWith("!", StringComparison.OrdinalIgnoreCase));

                        if (expectedUrlKeyPrefix == currentUrlKeyPrefix)
                        {
                            paramsMapper.ResolveUrlParams(originalParams, requestContext, modelUrlKeyPrefix);
                        }
                    }
                    else
                    {
                        paramsMapper.ResolveUrlParams(originalParams, requestContext);
                    }

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
                if (this.ShouldProcessRequest(controller))
                {
                    // in indexing mode, we only request pages, therefore there in no need to update data for relative routes
                    if (!proxyControl.IsIndexingMode())
                    {
                        if (FrontendManager.AttributeRouting.UpdateRouteData(this.Context, controller.RouteData))
                        {
                            RouteHelper.SetUrlParametersResolved();
                        }
                    }
                    else
                    {
                        //// Attribute routing was successful.
                        RouteHelper.SetUrlParametersResolved();
                    }
                }
            }
        }

        /// <summary>
        /// Logs exceptions thrown by the invocation of <see cref="ControllerActionInvoker"/>
        /// </summary>
        /// <param name="proxyControl">The proxy control.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void ExecuteController(MvcProxyBase proxyControl)
        {
            // Stop processing, personalized widgets are executed asynchronously
            if (proxyControl is PersonalizedWidgetProxy)
                return;

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
            // prevent sending sensitive security information to the client
            var cryptoException = err as System.Security.Cryptography.CryptographicException;
            if (cryptoException != null)
            {
                Thread.Sleep(0); // protect from timing attack 
                return;
            }

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

        internal static object GetModelProperty(ControllerBase controller, string propertyName)
        {
            object wrapper;
            var modelProperty = controller.GetType().GetProperty("Model");

            if (modelProperty != null)
            {
                wrapper = modelProperty.GetValue(controller, null);
            }
            else
            {
                wrapper = controller;
            }

            var providerNameProperty = wrapper.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.Name == propertyName);

            if (providerNameProperty != null)
            {
                return providerNameProperty.GetValue(wrapper, null);
            }

            return null;
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
                        var providerNames = GetModelProperty(controller, "ProviderName") as string;
                        result = result.SetLast(new DetailActionParamsMapper(controller, contentType, () => providerNames));
                    }
                }
            }

            return result;
        }

        private IUrlParamsMapper GetInferredSuccessorsActionParamsMapper(ControllerBase controller)
        {
            IUrlParamsMapper result = null;
            var controllerType = controller.GetType();
            if (controllerType.ImplementsInterface(typeof(ICanFilterByParent)))
            {
                var successorsAction = new ReflectedControllerDescriptor(controllerType).FindAction(controller.ControllerContext, SuccessorsActionParamsMapper.DefaultActionName);
                if (successorsAction != null)
                {
                    ICanFilterByParent canFilterByParent = controller as ICanFilterByParent;
                    var parentContentTypes = canFilterByParent.GetParentTypes();
                    if (parentContentTypes != null && parentContentTypes.Count() > 0)
                    {
                        var providerName = GetModelProperty(controller, "ProviderName") as string;
                        result = result.SetLast(new SuccessorsActionParamsMapper(controller, parentContentTypes, () => providerName));
                    }
                }
            }

            return result;
        }

        private IUrlParamsMapper GetInferredDateFilterMapper(ControllerBase controller, string actionName)
        {
            var actionDescriptor = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);

            if (actionDescriptor == null || actionDescriptor.GetParameters().Length == 0)
            {
                return null;
            }

            return new DateUrlParamsMapper(controller, new DateUrlEvaluatorAdapter());
        }

        private IUrlParamsMapper GetInferredTaxonFilterQueryParamsMapper(ControllerBase controller, string actionName)
        {
            IUrlParamsMapper result = null;

            var actionDescriptor = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);
            if (actionDescriptor == null || actionDescriptor.GetParameters().Length == 0)
            {
                return null;
            }

            var queryParams = controller.ControllerContext.RequestContext.HttpContext.Request.QueryString;
            if (actionDescriptor.GetParameters()[0].ParameterType == typeof(ITaxon) && queryParams.Count == 3)
            {
                if (queryParams.Keys.Contains("taxonomy"))
                {
                    result = new TaxonomyUrlParamsMapper(controller, new TaxonUrlMapper(new TaxonUrlEvaluatorAdapter()));
                }
            }

            return result;
        }

        private IUrlParamsMapper GetInferredTaxonFilterMapper(ControllerBase controller, string actionName)
        {
            var actionDescriptor = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);

            if (actionDescriptor == null || actionDescriptor.GetParameters().Length == 0)
                return null;

            var contentType = GetModelProperty(controller, "ContentType") as Type;
            if (contentType == null && controller is IDynamicContentWidget)
            {
                var dynamicType = controller.GetDynamicContentType();
                if (dynamicType != null)
                {
                    contentType = TypeResolutionService.ResolveType(dynamicType.GetFullTypeName());
                }
            }

            ISet<Guid> taxonomiesForType = null;
            if (contentType != null)
            {
                var properties = TypeDescriptor.GetProperties(contentType);
                taxonomiesForType = properties.OfType<TaxonomyPropertyDescriptor>().Select(x => x.TaxonomyId).ToHashSet();
            }

            if (taxonomiesForType != null && taxonomiesForType.Count == 0)
            {
                return null;
            }

            IUrlParamsMapper result = null;
            if (actionDescriptor.GetParameters()[0].ParameterType == typeof(ITaxon))
            {
                string routeTemplate = GenerateRouteTemplate(taxonomiesForType);
                var taxonParamName = actionDescriptor.GetParameters()[0].ParameterName;
                if (actionDescriptor.GetParameters()[1].ParameterType == typeof(int?))
                {
                    var pageParamName = actionDescriptor.GetParameters()[1].ParameterName;
                    var urlParamNames = new string[] { FeatherActionInvoker.TaxonNamedParamter, FeatherActionInvoker.PagingNamedParameter };
                    result = new CustomActionParamsMapper(controller, () => "/{" + taxonParamName + ":" + routeTemplate + "}/{" + pageParamName + ":int}", actionName, urlParamNames);
                }

                var urlTaxonParamNames = new string[] { FeatherActionInvoker.TaxonNamedParamter };
                result = result.SetLast(new CustomActionParamsMapper(controller, () => "/{" + taxonParamName + ":" + routeTemplate + "}", actionName, urlTaxonParamNames));
            }

            return result;
        }

        private string GenerateRouteTemplate(ISet<Guid> taxonomiesForType)
        {
            string routeTemplate = string.Empty;

            var taxonomies = TaxonomyManager.GetTaxonomiesCache();

            if (taxonomies.Count() > 0)
            {
                var taxonomyNames = new List<string>();
                foreach (var taxonomy in taxonomies)
                {
                    if (taxonomiesForType != null && !taxonomiesForType.Contains(taxonomy.Id))
                        continue;

                    if (taxonomy.Id == TaxonomyManager.TagsTaxonomyId)
                    {
                        taxonomyNames.Add("tag");
                    }
                    else if (taxonomy.Id == TaxonomyManager.CategoriesTaxonomyId)
                    {
                        taxonomyNames.Add("category");
                    }
                    else
                    {
                        taxonomyNames.Add(taxonomy.Name.ToLowerInvariant());
                    }
                }
                routeTemplate = string.Join(",", taxonomyNames);
            }

            return routeTemplate;
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

            var urlParamNames = new string[] { FeatherActionInvoker.PagingNamedParameter };

            return new CustomActionParamsMapper(controller, () => "/{" + actionDescriptor.GetParameters()[0].ParameterName + ":int}", actionName, urlParamNames);
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

        /// <summary>
        /// The name of the page route parameter
        /// </summary>
        internal const string PagingNamedParameter = "page";

        /// <summary>
        /// The name of the taxon route parameter
        /// </summary>
        internal const string TaxonNamedParamter = "taxon";
    }
}
