using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Mvc.Rendering;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Web.Configuration;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helper methods for registering client resources.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <remarks>
        /// This method uses directly the resource from the <see cref="scriptPath"/>. 
        /// In case you want to use embedded scripts from Sitefinity check <see cref="ResourceHelper.Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false)"/>.
        /// </remarks>
        /// <param name="scriptPath">The path to the JavaScript file.</param>
        /// <param name="throwException">OPTIONAL: Indicates whether to throw an exception if the JavaScript is already registered. By default the value is set to <value>false</value>.</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString Script(this HtmlHelper helper, string scriptPath, bool throwException = false)
        {
            var context = helper.ViewContext.HttpContext;

            return ResourceHelper.RegisterResource(context, scriptPath, scriptPath, throwException);
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <remarks>
        /// This helper references the same resource existing in Sitefinity.
        /// </remarks>
        /// <param name="helper">The helper.</param>
        /// <param name="scriptReference">The script reference.</param>
        /// <param name="throwException">if set to <c>true</c> throw exception.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false)
        {
            if (ResourceHelper.TryConfigureScriptManager(scriptReference))
                return MvcHtmlString.Empty;

            var resourceKey = scriptReference.ToString();
            var context = helper.ViewContext.HttpContext;
            var resourceUrl = ResourceHelper.GetWebResourceUrl(scriptReference);

            if (string.IsNullOrEmpty(resourceUrl))
                return MvcHtmlString.Empty;

            return ResourceHelper.RegisterResource(context, resourceKey, resourceUrl, throwException);
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <remarks>
        /// Use this helper to resolve embedded resource path.
        /// </remarks>
        /// <param name="helper">The helper.</param>
        /// <param name="type">The type.</param>
        /// <param name="embeddedScriptPath">The embedded script path.</param>
        /// <param name="throwException">if set to <c>true</c> throws exception.</param>
        /// <returns>MvcHtmlString</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString Script(this HtmlHelper helper, string type, string embeddedScriptPath, bool throwException = false)
        {
            var context = helper.ViewContext.HttpContext;
            var page = HttpContext.Current.Handler as Page ?? new PageProxy(null);

            var resourceUrl = page.ClientScript.GetWebResourceUrl(
                    TypeResolutionService.ResolveType(type),
                    embeddedScriptPath);

            return ResourceHelper.RegisterResource(context, resourceUrl, resourceUrl, throwException);
        }

        /// <summary>
        /// Tries the configure script manager.
        /// </summary>
        /// <param name="scriptReference">The script reference.</param>
        /// <returns></returns>
        private static bool TryConfigureScriptManager(ScriptRef scriptReference)
        {
            var page = HttpContext.Current.Handler as Page;
            ScriptManager scriptManager = null;

            if (page != null)
            {
                scriptManager = PageManager.ConfigureScriptManager(page, scriptReference, false);
            }

            if (scriptManager == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the web resource URL.
        /// </summary>
        /// <param name="scriptReference">The script reference.</param>
        /// <returns></returns>
        private static string GetWebResourceUrl(ScriptRef scriptReference)
        {
            var config = Config.Get<PagesConfig>().ScriptManager;
            var scriptConfig = config.ScriptReferences[scriptReference.ToString()];
            string resourceUrl = string.Empty;

            if (config.EnableCdn || (scriptConfig.EnableCdn.HasValue && scriptConfig.EnableCdn.Value))
            {
                resourceUrl = scriptConfig.Path;
            }
            else
            {
                var page = HttpContext.Current.Handler as Page ?? new PageProxy(null);

                resourceUrl = page.ClientScript.GetWebResourceUrl(
                    TypeResolutionService.ResolveType("Telerik.Sitefinity.Resources.Reference"),
                    scriptConfig.Name);
            }

            return resourceUrl;
        }

        /// <summary>
        /// Registers the resource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scriptKey">The script key.</param>
        /// <param name="scriptPath">The script path.</param>
        /// <param name="throwException">if set to <c>true</c> throws exception.</param>
        /// <returns></returns>
        private static MvcHtmlString RegisterResource(HttpContextBase context, string scriptKey, string scriptPath, bool throwException)
        {
            var attributes = new KeyValuePair<string, string>[2];
            attributes[0] = new KeyValuePair<string, string>("src", scriptPath);
            attributes[1] = new KeyValuePair<string, string>("type", "text/javascript");

            var register = new ResourceRegister(ResourceHelper.JsRegisterName, context);

            return ResourceHelper.RegisterResource(register, scriptKey, throwException, tagName: "script", attribbutes: attributes);
        }

        /// <summary>
        /// Registers resource reference.
        /// </summary>
        /// <param name="register">The register.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="attribbutes">The attribbutes.</param>
        /// <returns></returns>
        private static MvcHtmlString RegisterResource(ResourceRegister register, string resourceKey, bool throwException, string tagName, KeyValuePair<string, string>[] attribbutes)
        {
            string output;
            MvcHtmlString result;

            if (throwException)
            {
                register.RegisterResource(resourceKey);
                output = ResourceHelper.GenerateTag(tagName, attribbutes);
                result = new MvcHtmlString(output);
            }
            else if (register.TryRegisterResource(resourceKey))
            {
                output = ResourceHelper.GenerateTag(tagName, attribbutes);
                result = new MvcHtmlString(output);
            }
            else
            {
                result = MvcHtmlString.Empty;
            }

            return result;
        }

        /// <summary>
        /// Creates a string representation of a tag. 
        /// </summary>
        /// <param name="tag">The type of the HTML tag that would be generated for every registered resource.</param>
        /// <param name="attributes">The attributes associated with the tag.</param>
        /// <returns>The string representation of a tag.</returns>
        private static string GenerateTag(string tagName, params KeyValuePair<string, string>[] attribbutes)
        {
            var tag = new TagBuilder(tagName);

            if (attribbutes != null)
            {
                foreach (var attr in attribbutes)
                    tag.Attributes[attr.Key] = attr.Value;
            }

            return tag.ToString();
        }

        private const string JsRegisterName = "JsRegister";
    }
}