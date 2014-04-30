using System;
using System.Web.Mvc;
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
        /// Get the label with the specified key from the resource files.
        /// </summary>
        /// <param name="key">The key.</param>
        public static string Label(this WebViewPage page, string key)
        {
            var controller = LocalizationHelpers.GetController(page);
            var resClass = LocalizationHelpers.FindResourceStringClassType(controller);
            return Res.Get(resClass, key);
        }

        /// <summary>
        /// Get the label with the specified key from the resource files.
        /// </summary>
        /// <param name="key">The key.</param>
        public static string Label(this ViewPage page, string key)
        {
            var controller = LocalizationHelpers.GetController(page);
            var resClass = LocalizationHelpers.FindResourceStringClassType(controller);
            return Res.Get(resClass, key);
        }

        /// <summary>
        /// Get the label with the specified key from the resource files.
        /// </summary>
        /// <param name="controller">Controller that requests the resource.</param>
        /// <param name="key">The key.</param>
        public static string Label(IController controller, string key)
        {
            var resClass = LocalizationHelpers.FindResourceStringClassType(controller);
            return Res.Get(resClass, key);
        }

        private static IController GetController(WebViewPage page)
        {
            return page.ViewContext.Controller;
        }

        private static IController GetController(ViewPage page)
        {
            return page.ViewContext.Controller;
        }

        private static Type FindResourceStringClassType(IController controller)
        {
            LocalizationAttribute rcdAttribute = Attribute.GetCustomAttribute(controller.GetType(), typeof(LocalizationAttribute)) as LocalizationAttribute;

            if (rcdAttribute != null)
            {
                return rcdAttribute.ResourceClass;
            }
            else
            {
                return typeof(Labels);
            }
        }
    }
}
