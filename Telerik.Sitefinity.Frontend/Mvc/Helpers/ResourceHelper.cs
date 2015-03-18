using System;
using System.Collections.Generic;
using System.Linq;
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
        public static System.Web.Mvc.MvcHtmlString Script(this HtmlHelper helper, string scriptPath, bool throwException = false)
        {
            var context = helper.ViewContext.HttpContext;

            var scriptTag = ResourceHelper.RegisterResource(context, scriptPath, scriptPath, throwException);
            return scriptTag.IsNullOrEmpty() ? MvcHtmlString.Empty : MvcHtmlString.Create(scriptTag);
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
        public static System.Web.Mvc.MvcHtmlString Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false)
        {
            if (ResourceHelper.TryConfigureScriptManager(scriptReference))
                return System.Web.Mvc.MvcHtmlString.Empty;

            var context = helper.ViewContext.HttpContext;
            var sb = new StringBuilder();

            var references = PageManager.GetScriptReferences(scriptReference).Select(r => new MvcScriptReference(r));
            foreach (var script in references)
            {
                var resourceUrl = script.GetResourceUrl();
                var scriptTag = ResourceHelper.RegisterResource(context, resourceUrl, resourceUrl, throwException);
                if (!scriptTag.IsNullOrEmpty())
                    sb.Append(scriptTag);
            }

            return MvcHtmlString.Create(sb.ToString());
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
        public static System.Web.Mvc.MvcHtmlString Script(this HtmlHelper helper, string type, string embeddedScriptPath, bool throwException = false)
        {
            var context = helper.ViewContext.HttpContext;
            var page = HttpContext.Current.Handler as Page ?? new PageProxy(null);

            var resourceUrl = page.ClientScript.GetWebResourceUrl(
                    TypeResolutionService.ResolveType(type),
                    embeddedScriptPath);

            return MvcHtmlString.Create(ResourceHelper.RegisterResource(context, resourceUrl, resourceUrl, throwException));
        }

        /// <summary>
        /// Gets the web resource URL.
        /// </summary>
        /// <param name="scriptReference">The script reference.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetWebResourceUrl(ScriptRef scriptReference)
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
        /// Registers the resource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scriptKey">The script key.</param>
        /// <param name="scriptPath">The script path.</param>
        /// <param name="throwException">if set to <c>true</c> throws exception.</param>
        /// <returns></returns>
        private static string RegisterResource(HttpContextBase context, string scriptKey, string scriptPath, bool throwException)
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
        private static string RegisterResource(ResourceRegister register, string resourceKey, bool throwException, string tagName, KeyValuePair<string, string>[] attribbutes)
        {
            string output;

            if (throwException)
            {
                register.RegisterResource(resourceKey);
                output = ResourceHelper.GenerateTag(tagName, attribbutes);
            }
            else if (register.TryRegisterResource(resourceKey))
            {
                output = ResourceHelper.GenerateTag(tagName, attribbutes);
            }
            else
            {
                output = string.Empty;
            }

            return output;
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

        private class MvcScriptReference : ScriptReference
        {
            public MvcScriptReference(ScriptReference reference)
            {
                if (reference == null)
                    throw new ArgumentNullException("reference");

                this.Path = reference.Path;
                this.Assembly = reference.Assembly;
                this.Name = reference.Name;
            }

            public string GetResourceUrl()
            {
                return this.GetUrl(new ScriptManager(), zip: false);
            }
        }
    }
}