using System;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helpers for views that are related to localization.
    /// </summary>
    public static class LocalizationHelpers
    {
        /// <summary>
        /// Get the resource string with the specified key from the resource files.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="key">The key.</param>
        /// <param name="fallbackToKey">If true then if a resource is not found with the specified key the key is returned.</param>
        public static string Resource(this HtmlHelper helper, string key, bool fallbackToKey = false)
        {
            var controller = helper.ViewContext.Controller;
            return LocalizationHelpers.Resource(controller, helper.ViewContext.RouteData, key, fallbackToKey);
        }

        /// <summary>
        /// Get the resource string with the specified key from the resource files.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="key">The key</param>
        /// <param name="className">The class to search in</param>
        public static string Resource(this HtmlHelper helper, string key, string className)
        {
            string result;

            if (Res.TryGet(className, key, out result))
            {
                return result;
            }

            return "#ResourceNotFound: {0}, {1}#".Arrange(className, key);
        }

        /// <summary>
        /// Get the label with the specified key from the resource files.
        /// </summary>
        /// <param name="key">The key.</param>
        [Obsolete("Use the Html.Resource(key) instead")]
        public static string Label(this WebViewPage page, string key)
        {
            var controller = LocalizationHelpers.GetController(page);
            var resClass = LocalizationHelpers.FindResourceStringClassType(controller.GetType());
            return Res.Get(resClass, key, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get the label with the specified key from the resource files.
        /// </summary>
        /// <param name="key">The key.</param>
        [Obsolete("Use the Html.Resource(key) instead")]
        public static string Label(this ViewPage page, string key)
        {
            var controller = LocalizationHelpers.GetController(page);
            var resClass = LocalizationHelpers.FindResourceStringClassType(controller.GetType());
            return Res.Get(resClass, key, System.Globalization.CultureInfo.InvariantCulture);
        } 

        /// <summary>
        /// Get the label with the specified key from the resource files.
        /// </summary>
        /// <param name="controller">Controller that requests the resource.</param>
        /// <param name="key">The key.</param>
        /// <param name="fallbackToKey">If true then if a resource is not found with the specified key the key is returned.</param>
        private static string Resource(ControllerBase controller, RouteData routeData, string key, bool fallbackToKey)
        {
            var resClass = LocalizationHelpers.FindResourceStringClassType(controller.GetType());

            var widgetName = routeData != null ? routeData.Values["widgetName"] as string : null;
            if (!string.IsNullOrEmpty(widgetName))
            {
                var widget = FrontendManager.ControllerFactory.ResolveControllerType(widgetName);
                if (widget != null)
                {
                    var widgetResClass = LocalizationHelpers.FindResourceStringClassType(widget);
                    string res;
                    if (widgetResClass != null && Res.TryGet(widgetResClass.Name, key, null, out res))
                    {
                        return res;
                    }
                }
            }

            string result;
            if (Res.TryGet(resClass.Name, key, null, out result))
            {
                return result;
            }

            if (fallbackToKey)
            {
                return key;
            }
            
            return "#ResourceNotFound: {0}, {1}#".Arrange(resClass.Name, key);
        }

        private static IController GetController(WebViewPage page)
        {
            return page.ViewContext.Controller;
        }

        private static IController GetController(ViewPage page)
        {
            return page.ViewContext.Controller;
        }

        private static Type FindResourceStringClassType(Type controller)
        {
            var rcdAttribute = Attribute.GetCustomAttribute(controller, typeof(LocalizationAttribute)) as LocalizationAttribute;

            if (rcdAttribute != null)
            {
                return rcdAttribute.ResourceClass;
            }
            
            return typeof(Labels);
        }
    }
}
