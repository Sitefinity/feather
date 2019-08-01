using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI.ContentUI.Enums;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve specific content item by the URL.
    /// </summary>
    internal class DetailActionParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="itemType">Type of the item that is expected.</param>
        public DetailActionParamsMapper(ControllerBase controller, Type itemType)
            : this(controller, itemType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="itemType">Type of the item that is expected.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        public DetailActionParamsMapper(ControllerBase controller, Type itemType, Func<string> providerNameResolver)
            : this(controller, itemType, providerNameResolver, DetailActionParamsMapper.DefaultActionName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="itemType">Type of the item that is expected.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentException">When the given controller does not contain a method corresponding to the action name.</exception>
        public DetailActionParamsMapper(ControllerBase controller, Type itemType, Func<string> providerNameResolver, string actionName)
            : base(controller)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType");

            this.actionName = actionName;
            this.providerNameResolver = providerNameResolver;

            this.ActionMethod = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);
            if (this.ActionMethod == null)
                throw new ArgumentException("The controller {0} does not have action '{1}'.".Arrange(controller.GetType().Name, actionName));

            this.ItemType = itemType;
        }

        private bool TryMatchUrl(string[] urlParams, RequestContext requestContext, bool setUrlParametersResolved)
        {
            if (urlParams == null || urlParams.Length == 0 || !this.IsDetailsModeSupported())
                return false;

            var url = RouteHelper.GetUrlParameterString(urlParams);
            string redirectUrl;
            var providerName = this.providerNameResolver != null ? this.providerNameResolver() : null;
            var contentItemResolver = new ContentDataItemResolver();
            var item = contentItemResolver.GetItemByUrl(url, this.ItemType, providerName, out redirectUrl);

            if (item != null)
            {
                SystemManager.CurrentHttpContext.Items["detailItem"] = item;

                this.AddContentItemToRouteData(requestContext, redirectUrl, item, setUrlParametersResolved);

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext, string urlKeyPrefix)
        {
            return this.TryMatchUrl(urlParams, requestContext, true);
        }

        /// <summary>
        /// Adds the content item to route data.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="item">The item.</param>
        /// <param name="setUrlParametersResolved">Whether to set the url parameters resolved</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        protected virtual void AddContentItemToRouteData(RequestContext requestContext, string redirectUrl, IDataItem item, bool setUrlParametersResolved = true)
        {
            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;

            var parameters = this.ActionMethod.GetParameters();
            if (parameters.Length > 0 && parameters[0].ParameterType.IsAssignableFrom(this.ItemType))
            {
                requestContext.RouteData.Values[parameters[0].ParameterName] = item;
            }

            if (!redirectUrl.IsNullOrEmpty())
            {
                requestContext.RouteData.Values[Telerik.Sitefinity.Mvc.ControllerActionInvoker.ShouldProcessRequestKey] = redirectUrl;
            }

            if (redirectUrl.IsNullOrEmpty() == false && parameters.Length > 1 && parameters[1].ParameterType == typeof(string))
            {
                requestContext.RouteData.Values[parameters[1].ParameterName] = redirectUrl;
            }

            if (setUrlParametersResolved)
            {
                RouteHelper.SetUrlParametersResolved();
            }
        }

        /// <summary>
        /// Check if the widget is configured to support details mode.
        /// </summary>
        protected bool IsDetailsModeSupported()
        {
            var modelProperty = this.Controller.GetType().GetProperty("Model");
            if (modelProperty != null)
            {
                var model = modelProperty.GetValue(this.Controller, null);
                var contentViewDisplayModeProperty = model == null ? null : model.GetType().GetProperty("ContentViewDisplayMode");
                if (contentViewDisplayModeProperty != null)
                {
                    var contentViewDisplayModeValue = contentViewDisplayModeProperty.GetValue(model, null);
                    if (contentViewDisplayModeValue != null && contentViewDisplayModeValue.ToString() == ContentViewDisplayMode.Master.ToString())
                        return false;
                }
            }

            return true;
        }

        protected Type ItemType { get; set; }

        protected ActionDescriptor ActionMethod { get; private set; }

        private string actionName;

        private Func<string> providerNameResolver;

        /// <summary>
        /// The default details action name.
        /// </summary>
        public const string DefaultActionName = "Details";
    }

    /// <summary>
    /// Instances of this class resolve specific content item by the URL.
    /// </summary>
    /// <typeparam name="TContent">The type of the content that should be resolved.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "It's the same class. This one is generic for convenience.")]
    internal class DetailActionParamsMapper<TContent> : DetailActionParamsMapper
        where TContent : class, IDataItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper{TContent}"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public DetailActionParamsMapper(ControllerBase controller)
            : base(controller, typeof(TContent))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper{TContent}"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        /// <exception cref="System.ArgumentException">When the given controller does not contain a method corresponding to the action name.</exception>
        public DetailActionParamsMapper(ControllerBase controller, Func<string> providerNameResolver)
            : base(controller, typeof(TContent), providerNameResolver, DetailActionParamsMapper.DefaultActionName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper{TContent}"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentException">When the given controller does not contain a method corresponding to the action name.</exception>
        public DetailActionParamsMapper(ControllerBase controller, Func<string> providerNameResolver, string actionName)
            : base(controller, typeof(TContent), providerNameResolver, actionName)
        {
        }
    }
}
