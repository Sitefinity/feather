using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve specific content item by the URL.
    /// </summary>
    public class DetailActionParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="itemType">Type of the item that is expected.</param>
        public DetailActionParamsMapper(Controller controller, Type itemType)
            : this(controller, itemType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="itemType">Type of the item that is expected.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        public DetailActionParamsMapper(Controller controller, Type itemType, Func<string> providerNameResolver)
            : this(controller, itemType, providerNameResolver, "Details")
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
        public DetailActionParamsMapper(Controller controller, Type itemType, Func<string> providerNameResolver, string actionName)
            : base(controller)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType");

            this.actionName = actionName;
            this.providerNameResolver = providerNameResolver;

            this.ActionMethod = this.Controller.GetType().GetMethod(actionName, BindingFlags.Public | BindingFlags.Instance);
            if (this.ActionMethod == null)
                throw new ArgumentException("The controller {0} does not have action '{1}'.".Arrange(controller.GetType().Name, actionName));

            this.ItemType = itemType;
            this.Manager = this.ResolveManager();
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext)
        {
            if (urlParams == null)
                return false;

            var url = RouteHelper.GetUrlParameterString(urlParams);
            string redirectUrl;
            var item = this.GetItemByUrl(url, out redirectUrl);

            if (item != null)
            {
                this.AddContentItemToRouteData(requestContext, redirectUrl, item);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the content item to route data.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="item">The item.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        protected virtual void AddContentItemToRouteData(RequestContext requestContext, string redirectUrl, IDataItem item)
        {
            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;

            var parameters = this.ActionMethod.GetParameters();
            if (parameters.Length > 0 && parameters[0].ParameterType.IsAssignableFrom(this.ItemType))
            {
                requestContext.RouteData.Values[parameters[0].Name] = item;
            }

            if (redirectUrl.IsNullOrEmpty() == false && parameters.Length > 1 && parameters[1].ParameterType == typeof(string))
            {
                requestContext.RouteData.Values[parameters[1].Name] = redirectUrl;
            }

            RouteHelper.SetUrlParametersResolved();
        }

        /// <summary>
        /// Gets content item by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        protected virtual IDataItem GetItemByUrl(string url, out string redirectUrl)
        {
            if (this.Manager == null)
            {
                redirectUrl = null;
                return null;
            }

            var itemIdFromQueryParam = (SystemManager.CurrentHttpContext.Request.Params["sf-itemId"] ?? SystemManager.CurrentHttpContext.Items["sf-itemId"]) as string;
            if (!itemIdFromQueryParam.IsNullOrEmpty())
            {
                Guid itemIdGuid;
                if (Guid.TryParse(itemIdFromQueryParam, out itemIdGuid))
                {
                    redirectUrl = null;
                    return this.Manager.GetItem(this.ItemType, itemIdGuid) as IDataItem;
                }
            }

            IDataItem item;
            if (this.Manager is IContentManager)
            {
                var isPublished = !this.IsPreviewRequested() ||
                    this.ResolveRequestedItemStatus() == ContentLifecycleStatus.Live;
                item = ((IContentManager)this.Manager).GetItemFromUrl(this.ItemType, url, isPublished, out redirectUrl);
            }
            else
            {
                item = ((IUrlProvider)this.Manager.Provider).GetItemFromUrl(this.ItemType, url, out redirectUrl);
            }

            var lifecycleItem = item as ILifecycleDataItem;
            if (lifecycleItem != null && this.Manager is ILifecycleManager)
            {
                object requestedItem;
                if (ContentLocatableViewExtensions.TryGetItemWithRequestedStatus(lifecycleItem, (ILifecycleManager)this.Manager, out requestedItem))
                {
                    item = requestedItem as IDataItem;
                }
            }

            return item;
        }

        /// <summary>
        /// Resolves the manager.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Items of the {0} type cannot be found by URL..Arrange(this.ItemType.FullName)</exception>
        protected IManager ResolveManager()
        {
            var providerName = this.providerNameResolver != null ? this.providerNameResolver() : null;
            var manager = ManagerBase.GetMappedManager(this.ItemType, providerName);

            if (!(manager is IContentManager) && !(manager.Provider is IUrlProvider))
                throw new InvalidOperationException("Items of the {0} type cannot be found by URL.".Arrange(this.ItemType.FullName));

            return manager;
        }

        /// <summary>
        /// This method returns the requested item status based on content location url parameters.
        /// </summary>
        /// <remarks>Copied from IContentLocatableView in Sitefinity. Should use it instead when it goes public.</remarks>
        /// <returns>Requested item status.</returns>
        private ContentLifecycleStatus ResolveRequestedItemStatus()
        {
            var itemStatusParam = SystemManager.CurrentHttpContext.Request.Params["sf-lc-status"] ?? SystemManager.CurrentHttpContext.Items["sf-lc-status"];
            ContentLifecycleStatus status = ContentLifecycleStatus.Live;
            if (itemStatusParam != null)
            {
                if (!Enum.TryParse<ContentLifecycleStatus>(itemStatusParam as string, out status))
                    status = ContentLifecycleStatus.Live;
            }

            return status;
        }

        /// <summary>
        /// Determines whether preview of the item is requested.
        /// </summary>
        /// <remarks>Copied from IContentLocatableView in Sitefinity. Should use it instead when it goes public.</remarks>
        /// <returns>Whether preview is requested.</returns>
        private bool IsPreviewRequested()
        {
            var actionParam = SystemManager.CurrentHttpContext.Request.Params["sf-content-action"];
            bool isPreview = actionParam != null && actionParam == "preview";
            return isPreview;
        }

        protected Type ItemType { get; set; }

        protected IManager Manager { get; private set; }

        protected MethodInfo ActionMethod { get; private set; }

        private string actionName;

        private Func<string> providerNameResolver;
    }

    /// <summary>
    /// Instances of this class resolve specific content item by the URL.
    /// </summary>
    /// <typeparam name="TContent">The type of the content that should be resolved.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "It's the same class. This one is generic for convenience.")]
    public class DetailActionParamsMapper<TContent> : DetailActionParamsMapper
        where TContent : class, IDataItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper{TContent}"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public DetailActionParamsMapper(Controller controller) : base(controller, typeof(TContent))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper{TContent}"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentException">When the given controller does not contain a method corresponding to the action name.</exception>
        public DetailActionParamsMapper(Controller controller, Func<string> providerNameResolver, string actionName = "Details")
            : base(controller, typeof(TContent), providerNameResolver, actionName)
        {
        }
    }
}
