﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.ContentLocations;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Mvc.Proxy.Security;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// The class contains helper methods related to resolving detail content items operations.
    /// </summary>
    public class ContentDataItemResolver
    {
        /// <summary>
        /// Gets the detail content item.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="routeParams">Contains information about the route parameters.</param>
        public IDataItem GetItemByController(ControllerBase controller, string[] routeParams)
        {
            string redirectUrl = null;
            IDataItem item = null;

            var url = RouteHelper.GetUrlParameterString(routeParams);

            var itemType = this.GetItemType(controller);
            if (itemType != null)
            {
                var providerNames = this.GetProviderNames(controller, itemType);
                foreach (var providerName in providerNames)
                {
                    item = this.GetItemByUrl(url, itemType, providerName, out redirectUrl);
                    if (item != null)
                    {
                        return item;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the content item by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="itemType">The content type of the item.</param>
        /// <param name="provider">The provider of the item</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        public IDataItem GetItemByUrl(string url, Type itemType, string provider, out string redirectUrl)
        {
            var manager = this.ResolveManager(itemType, provider);
            if (manager == null)
            {
                redirectUrl = null;
                return null;
            }

            var itemIdFromQueryParam = (SystemManager.CurrentHttpContext.Request.ParamsGet("sf-itemId") ?? SystemManager.CurrentHttpContext.Items["sf-itemId"]) as string;
            if (!itemIdFromQueryParam.IsNullOrEmpty())
            {
                Guid itemIdGuid;
                if (Guid.TryParse(itemIdFromQueryParam, out itemIdGuid))
                {
                    redirectUrl = null;
                    return manager.GetItem(itemType, itemIdGuid) as IDataItem;
                }
            }

            IDataItem item = null;
            if (manager is IContentManager || manager is DynamicModuleManager)
            {
                try
                {
                    item = this.GetItemFromUrl(manager, itemType, url, out redirectUrl);
                }
                catch (System.UnauthorizedAccessException e)
                {
                    redirectUrl = null;
                    ItemAccessException access = new ItemAccessException(e.Message, e.InnerException);
                    access.Data["ItemType"] = itemType.Name;

                    throw access;
                }

                if (item != null)
                {
                    var type = item.GetType();
                    if (!type.Equals(itemType) && !(type.Module != null && type.Module.ScopeName == "OpenAccessDynamic" && type.BaseType != null && type.BaseType.Equals(itemType)))
                    {
                        item = null;
                    }
                }
            }
            else
            {
                item = ((IUrlProvider)manager.Provider).GetItemFromUrl(itemType, url, out redirectUrl);
            }

            var lifecycleItem = item as ILifecycleDataItem;
            var lifecycleManager = manager as ILifecycleManager;
            if (lifecycleItem != null && lifecycleManager != null)
            {
                ContentLifecycleStatus requestedStatus = ContentLocatableViewExtensions.GetRequestedItemStatus();
                if (lifecycleItem.Status != requestedStatus)
                {
                    item = lifecycleManager.Lifecycle.GetLive(lifecycleItem);
                }

                object requestedItem;
                if (ContentLocatableViewExtensions.TryGetItemWithRequestedStatus(lifecycleItem, lifecycleManager, out requestedItem))
                {
                    item = requestedItem as IDataItem;
                }
            }

            return item;
        }

        private IDataItem GetItemFromUrl(IManager manager, Type itemType, string url, out string redirectUrl)
        {
            IDataItem item = null;
            var isPublished = !this.IsPreviewRequested() || this.ResolveRequestedItemStatus() == ContentLifecycleStatus.Live;

            if (manager is IContentManager)
            {
                var contentManager = manager as IContentManager;
                item = contentManager.GetItemFromUrl(itemType, url, isPublished, out redirectUrl);
            }
            else
            {
                var dynamicManager = manager as DynamicModuleManager;
                item = dynamicManager.Provider.GetItemFromUrl(itemType, url, isPublished, out redirectUrl);
            }

            return item;
        }

        /// <summary>
        /// Gets the type of the content item for specified controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        private Type GetItemType(ControllerBase controller)
        {
            var controllerType = controller.GetType();
            Type contentType = null;
            var detailsAction = new ReflectedControllerDescriptor(controllerType).FindAction(controller.ControllerContext, DetailActionParamsMapper.DefaultActionName);
            if (detailsAction != null)
            {
                var contentParam = detailsAction.GetParameters().FirstOrDefault();
                if (contentParam != null && contentParam.ParameterType.ImplementsInterface(typeof(IDataItem)))
                {
                    if (typeof(DynamicContent) == contentParam.ParameterType)
                    {
                        var controllerName = this.GetControllerName(controller);
                        var dynamicContentType = controller.GetDynamicContentType(controllerName);
                        contentType = dynamicContentType != null ? TypeResolutionService.ResolveType(dynamicContentType.GetFullTypeName(), throwOnError: false) : null;
                    }
                    else
                    {
                        contentType = contentParam.ParameterType;
                    }
                }
            }

            return contentType;
        }

        private string GetControllerName(ControllerBase controller)
        {
            var widgetName = (string)controller.ViewBag.WidgetName;

            if (!string.IsNullOrEmpty(widgetName))
            {
                return widgetName;
            }

            var controllerName = controller.GetType().Name.Replace("Controller", "");
            return controllerName;
        }

        /// <summary>
        /// This method returns the requested item status based on content location url parameters.
        /// </summary>
        /// <remarks>Copied from IContentLocatableView in Sitefinity. Should use it instead when it goes public.</remarks>
        /// <returns>Requested item status.</returns>
        private ContentLifecycleStatus ResolveRequestedItemStatus()
        {
            var itemStatusParam = SystemManager.CurrentHttpContext.Request.ParamsGet("sf-lc-status") ?? SystemManager.CurrentHttpContext.Items["sf-lc-status"];
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
            var actionParam = SystemManager.CurrentHttpContext.Request.ParamsGet("sf-content-action");
            bool isPreview = actionParam != null && actionParam == "preview";
            return isPreview;
        }

        /// <summary>
        /// Resolves the manager.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private IManager ResolveManager(Type itemType, string providerName)
        {
            IManager manager;

            try
            {
                ManagerBase.TryGetMappedManager(itemType, providerName, out manager);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Exception occurred in the routing functionality, details: {0}", ex));
                manager = null;
            }

            return manager;
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
                    IList<string> providers = manager.GetContextProviders().Select(p => p.Name).ToList();

                    if (!providers.Contains(manager.Provider.Name))
                        providers.Add(manager.Provider.Name);

                    return providers;
                }
                else
                {
                    return new string[0];
                }
            }
        }
    }
}
