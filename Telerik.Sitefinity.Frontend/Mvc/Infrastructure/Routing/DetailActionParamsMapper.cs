﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.DataResolving;
using Telerik.Sitefinity.Web.UI.ContentUI.Enums;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve specific content item by the URL.
    /// </summary>
    internal class DetailActionParamsMapper : UrlParamsMapperBase
    {
        private bool showDetailsViewOnChildDetailsView;

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

            var value = FeatherActionInvoker.GetModelProperty(controller, "ShowDetailsViewOnChildDetailsView");
            if (value != null && value is bool)
            {
                this.showDetailsViewOnChildDetailsView = (bool)value;
            }
        }

        private bool TryMatchUrl(string[] urlParams, RequestContext requestContext, bool setUrlParametersResolved, bool setToContext = true)
        {
            if (urlParams == null || urlParams.Length == 0 || !this.IsDetailsModeSupported())
                return false;

            var url = RouteHelper.GetUrlParameterString(urlParams);
            string redirectUrl;
            var providerName = this.providerNameResolver != null ? this.providerNameResolver() : null;
            var contentItemResolver = new ContentDataItemResolver();
            var item = contentItemResolver.GetItemByUrl(url, this.ItemType, providerName, out redirectUrl);

            if (item != null && this.CanDisplayItem(item))
            {
                if(setToContext)
                    SystemManager.CurrentHttpContext.Items["detailItem"] = item;

                this.AddContentItemToRouteData(requestContext, redirectUrl, item, setUrlParametersResolved);

                return true;
            }
            else if (this.showDetailsViewOnChildDetailsView)
            {
                return this.TryMatchUrl(urlParams.Take(urlParams.Length - 1).ToArray(), requestContext, setUrlParametersResolved, false);
            }

            return false;
        }

        /// <summary>
        /// Determines whether an item should be displayed depending if the item or it`s parent is selected or the the item does not have any filtration
        /// </summary>
        /// <param name="item">Data item</param>
        /// <returns>Return true if the item is selected, has a selected parent or does not have any filtration, else return false</returns>
        private bool CanDisplayItem(IDataItem item)
        {
            bool canDispay = true;

            var modelProperty = this.Controller.GetType().GetProperty("Model");
            if (modelProperty != null)
            {
                var model = modelProperty.GetValue(this.Controller);
                if (model != null)
                {
                    if (this.IsPreviewRequested())
                        item = this.TryGetLiveItem(item);
                    
                    string serializedSelectedItemsIds = this.GetSelectedItemsIds(model);
                    if (serializedSelectedItemsIds != null)
                    {
                        canDispay = this.IsSelectedItem(item, model, serializedSelectedItemsIds);
                    }
                    else
                    {
                        if (this.IsParentFilterModeSelected(model))
                        {
                            string serializedSelectedParentsItemsIds = this.GetSelectedParentsIds(model);
                            if (serializedSelectedParentsItemsIds != null)
                            {
                                canDispay = this.HasSelectedParentItem(item, model, serializedSelectedParentsItemsIds);
                            }
                        }
                    }
                }
            }

            return canDispay;
        }

        /// <summary>
        /// Checks to see if the item given is the master one. If true returns the live item.
        /// </summary>
        /// <param name="item">Data item</param>
        /// <returns></returns>
        private IDataItem TryGetLiveItem(IDataItem item)
        {
            if (item is ILifecycleDataItem itemAsLifecycle)
            {
                if (itemAsLifecycle.Status == ContentLifecycleStatus.Master)
                {
                    var manager = ManagerBase.GetMappedManager(item.GetType(), item.Provider.ToString());
                    var lifecycleManager = manager as ILifecycleManager;

                    var liveItem = lifecycleManager.Lifecycle.GetLive(itemAsLifecycle);
                    if (liveItem != null)
                    {
                        return liveItem;
                    }
                }
            }

            return item;
        }

        private bool IsPreviewRequested()
        {
            var actionParam = SystemManager.CurrentHttpContext.Request.ParamsGet("sf-content-action");
            bool isPreview = actionParam != null && actionParam == "preview";

            return isPreview;
        }

        /// <summary>
        /// Determines whether the item is selected
        /// </summary>
        /// <param name="item">Data item</param>
        /// <param name="model">The widget model</param>
        /// <returns>Return false if item is not in the selected items, else return true</returns>
        private bool IsSelectedItem(IDataItem item, object model, string serializedSelectedItemsIds)
        {
            bool isSelectedItem = true;

            var selectionModeProperty = model.GetType().GetProperty("SelectionMode");
            if (selectionModeProperty != null)
            {
                var selectionMode = selectionModeProperty.GetValue(model);
                if (selectionMode != null)
                {
                    if (selectionMode.ToString() == SelectionMode.SelectedItems.ToString())
                    {
                        isSelectedItem = serializedSelectedItemsIds.Contains(item.Id.ToString());
                        if (!isSelectedItem)
                        {
                            if (item is ILifecycleDataItemGeneric itemAsLifecycleGeneric)
                            {
                                isSelectedItem = serializedSelectedItemsIds.Contains(itemAsLifecycleGeneric.OriginalContentId.ToString());
                            }
                            else if (item is SitefinityProfile profile)
                            {
                                isSelectedItem = serializedSelectedItemsIds.Contains(profile.User.Id.ToString());
                            }
                        }
                    }
                    // This is a special case for the list item as it`s parent id is stored in the selected items property
                    else if (item is IHasParent)
                    {
                        isSelectedItem = serializedSelectedItemsIds.Contains((item as IHasParent).Parent.Id.ToString());
                    }
                }
            }

            return isSelectedItem;
        }

        /// <summary>
        /// Determines whether ParentFilterMode property is set to Selected
        /// </summary>
        /// <param name="model">The widget model</param>
        /// <returns>Return true if the ParentFilterMode is set to Selected, else return false</returns>
        private bool IsParentFilterModeSelected(object model)
        {
            var parentFilterModeProperty = model.GetType().GetProperty("ParentFilterMode");
            if (parentFilterModeProperty != null)
            {
                var parentFilterMode = parentFilterModeProperty.GetValue(model);
                if (parentFilterMode != null && parentFilterMode.ToString() == "Selected")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the selected items ids
        /// </summary>
        /// <param name="model">The widget model</param>
        /// <returns>The selected items ids as a string</returns>
        private string GetSelectedItemsIds(object model)
        {
            var serializedSelectedItemsIdsProperty = model.GetType().GetProperty("SerializedSelectedItemsIds");

            if (serializedSelectedItemsIdsProperty != null)
            {
                var serializedSelectedItemsIds = serializedSelectedItemsIdsProperty.GetValue(model);
                if (serializedSelectedItemsIds != null && serializedSelectedItemsIds.ToString() != "[]")
                {
                    return serializedSelectedItemsIds.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the item has a selected parent
        /// </summary>
        /// <param name="item">Data item</param>
        /// <param name="model">The widget model</param>
        /// <param name="serializedSelectedParentsIds">The selected parets ids</param>
        /// <returns>Return false if the item`s parent is not selected, else return true</returns>
        private bool HasSelectedParentItem(IDataItem item, object model, string serializedSelectedParentsIds)
        {
            bool hasSelectedParentItem = true;

            // The parent id is retrieved differently depending if it implement IHasParent or is Dynamic content
            if (item is IHasParent)
            {
                hasSelectedParentItem = serializedSelectedParentsIds.ToString().Contains((item as IHasParent).Parent.Id.ToString());
                if (!hasSelectedParentItem)
                {
                    // If the item is saved in a folder, e.g. document, images, videos, the id of the folder corresponds with the parent id
                    var folderIdProperty = item.GetType().GetProperty("FolderId");
                    if (folderIdProperty != null)
                    {
                        var folderId = folderIdProperty.GetValue(item);
                        if (folderId != null)
                        {
                            hasSelectedParentItem = serializedSelectedParentsIds.ToString().Contains(folderId.ToString());
                        }
                    }
                }
            }
            else if (item is DynamicContent)
            {
                hasSelectedParentItem = serializedSelectedParentsIds.ToString().Contains((item as DynamicContent).SystemParentId.ToString());
            }

            return hasSelectedParentItem;
        }

        /// <summary>
        /// Gets the selected parents ids
        /// </summary>
        /// <param name="model">The widget model</param>
        /// <returns>The selected parents ids as a string</returns>
        private string GetSelectedParentsIds(object model)
        {
            var serializedSelectedParentsIdsProperty = model.GetType().GetProperty("SerializedSelectedParentsIds");

            if (serializedSelectedParentsIdsProperty != null)
            {
                var serializedSelectedParentsIds = serializedSelectedParentsIdsProperty.GetValue(model);
                if (serializedSelectedParentsIds != null && serializedSelectedParentsIds.ToString() != "[]")
                {
                    return serializedSelectedParentsIds.ToString();
                }
            }

            return null;
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
                var node = SiteMapBase.GetCurrentProvider().CurrentNode as PageSiteNode;
                string urlKeyPrefix = string.Empty;
                var modelProperty = this.Controller.GetType().GetProperty("Model");
                if (modelProperty != null)
                {
                    var model = modelProperty.GetValue(this.Controller, null);
                    if (model != null)
                    {
                        var urlKeyPrefixProperty = model.GetType().GetProperty("UrlKeyPrefix");
                        if (urlKeyPrefixProperty != null)
                        {
                            urlKeyPrefix = (string)urlKeyPrefixProperty.GetValue(model);
                        }
                    }
                }

                var resolvedUrl = DataResolver.Resolve(item, "URL", urlKeyPrefix, node.Id.ToString());

                if (requestContext.HttpContext.Request.QueryString.Count > 0)
                {
                    resolvedUrl = $"{resolvedUrl}?{requestContext.HttpContext.Request.QueryString}";
                }

                requestContext.RouteData.Values[Telerik.Sitefinity.Mvc.ControllerActionInvoker.SfRedirectUrlKey] = resolvedUrl;
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
