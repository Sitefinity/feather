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
        /// Renders all scripts.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string RenderAllScripts(HttpContextBase context)
        {
            var scriptRegister = new ResourceRegister(ResourceHelper.JsRegisterName, context);
            var scriptMarkup = ResourceHelper.BuildHtmlResourcesMarkup(scriptRegister, ResourceType.Js);

            return scriptMarkup;
        }

        /// <summary>
        /// Renders all stylesheets.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string RenderAllStylesheets(HttpContextBase context)
        {
            var stylesheetRegister = new ResourceRegister(ResourceHelper.CssRegisterName, context);
            var stylesheetMarkup = ResourceHelper.BuildHtmlResourcesMarkup(stylesheetRegister, ResourceType.Css);

            return stylesheetMarkup;
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="scriptPath">The path to the JavaScript file.</param>
        /// <param name="throwException">OPTIONAL: Indicates whether to throw an exception if the JavaScript is already registered. By default the value is set to <value>false</value>.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        /// <remarks>
        /// This method uses directly the resource from the <see cref="scriptPath" />.
        /// In case you want to use embedded scripts from Sitefinity check <see cref="ResourceHelper.Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false)" />.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString Script(this HtmlHelper helper, string scriptPath, bool throwException = false, string sectionName = null)
        {
            if (ResourceHelper.TryConfigureScriptManager(scriptPath))
                return MvcHtmlString.Empty;

            return ResourceHelper.InnerRegisterResource(helper.ViewContext.HttpContext, scriptPath, ResourceType.Js, throwException, sectionName);
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
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false, string sectionName = null)
        {
            if (ResourceHelper.TryConfigureScriptManager(scriptReference))
                return System.Web.Mvc.MvcHtmlString.Empty;

            var references = PageManager.GetScriptReferences(scriptReference).Select(r => new MvcScriptReference(r));

            StringBuilder outputMarkup = new StringBuilder();

            foreach (var script in references)
            {
                var resourceUrl = script.GetResourceUrl();
                outputMarkup.Append(ResourceHelper.InnerRegisterResource(helper.ViewContext.HttpContext, resourceUrl, ResourceType.Js, throwException, sectionName));
            }

            return MvcHtmlString.Create(outputMarkup.ToString());
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
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString Script(this HtmlHelper helper, string type, string embeddedScriptPath, bool throwException = false, string sectionName = null)
        {
            var page = HttpContext.Current.Handler as Page ?? new PageProxy(null);
            var resourceUrl = page.ClientScript.GetWebResourceUrl(TypeResolutionService.ResolveType(type), embeddedScriptPath);

            if (ResourceHelper.TryConfigureScriptManager(resourceUrl))
                return MvcHtmlString.Empty;

            return ResourceHelper.InnerRegisterResource(helper.ViewContext.HttpContext, resourceUrl, ResourceType.Js, throwException, sectionName);
        }

        /// <summary>
        /// Registers style sheet reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="resourcePath">The path to the CSS file.</param>
        /// <param name="throwException">OPTIONAL: Indicates whether to throw an exception if the CSS is already registered. By default the value is set to <value>false</value>.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString StyleSheet(this HtmlHelper helper, string resourcePath, bool throwException = false, string sectionName = null)
        {
            return ResourceHelper.InnerRegisterResource(helper.ViewContext.HttpContext, resourcePath, ResourceType.Css, throwException, sectionName);
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

        private static MvcHtmlString InnerRegisterResource(HttpContextBase httpContext, string resourcePath, ResourceType resourceType, bool throwException, string sectionName)
        {
            var registerName = string.Empty;
            if (resourceType == ResourceType.Js)
                registerName = ResourceHelper.JsRegisterName;
            else if (resourceType == ResourceType.Css)
                registerName = ResourceHelper.CssRegisterName;

            var register = new ResourceRegister(registerName, httpContext);

            MvcHtmlString result = MvcHtmlString.Empty;

            // No section name renders the script inline if it hasn't been rendered
            if (string.IsNullOrEmpty(sectionName))
            {
                if (!register.IsResourceRegistered(resourcePath, sectionName: null))
                {
                    result = MvcHtmlString.Create(ResourceHelper.BuildResourceMarkup(register, resourcePath, resourceType));
                }
            }

            // Register the resource even if it had to be rendered inline (avoid repetitions).
            register.RegisterResource(resourcePath, sectionName, throwException);

            return result;
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
        /// Tries the configure script manager.
        /// </summary>
        /// <param name="scriptReference">The script reference.</param>
        /// <returns></returns>
        private static bool TryConfigureScriptManager(string scriptReference)
        {
            var page = HttpContext.Current.Handler as Page;
            ScriptManager scriptManager = null;

            if (page != null)
            {
                scriptManager = ScriptManager.GetCurrent(page);
            }

            if (scriptManager != null)
            {
                scriptManager.Scripts.Add(new ScriptReference(scriptReference));
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string BuildHtmlResourcesMarkup(ResourceRegister resourceRegister, ResourceType resourceType)
        {
            StringBuilder output = new StringBuilder();

            foreach (var resourceInfo in resourceRegister.Container)
            {
                output.Append(ResourceHelper.BuildResourceMarkup(resourceRegister, resourceInfo.Key, resourceType));
            }

            return output.ToString();
        }

        private static string BuildResourceMarkup(ResourceRegister resourceRegister, string resourceKey, ResourceType resourceType)
        {
            string result = string.Empty;

            if (resourceType == ResourceType.Js)
                result = BuildScriptMarkup(resourceRegister, resourceKey);
            else if (resourceType == ResourceType.Css)
                result = BuildStyleSheetMarkup(resourceRegister, resourceKey);

            return result;
        }

        private static string BuildScriptMarkup(ResourceRegister resourceRegister, string resourceKey)
        {
            var resourceHtml = string.Empty;
            if (resourceRegister.Container[resourceKey] != null)
            {
                var attributes = new KeyValuePair<string, string>[2];
                attributes[0] = new KeyValuePair<string, string>("src", resourceKey);
                attributes[1] = new KeyValuePair<string, string>("type", "text/javascript");

                resourceHtml = ResourceHelper.GenerateTag("script", attributes);
                // resourceRegister.Container[resourceKey] = true;
            }

            return resourceHtml;
        }

        private static string BuildStyleSheetMarkup(ResourceRegister resourceRegister, string resourceKey)
        {
            var resourceHtml = string.Empty;

            if (resourceRegister.Container[resourceKey] != null)
            {
                var attributes = new KeyValuePair<string, string>[3];
                attributes[0] = new KeyValuePair<string, string>("rel", "stylesheet");
                attributes[1] = new KeyValuePair<string, string>("href", resourceKey);
                attributes[2] = new KeyValuePair<string, string>("type", "text/css");

                resourceHtml = ResourceHelper.GenerateTag("link", attributes);
                // resourceRegister.Container[resourceKey] = true;
            }

            return resourceHtml;
        }

        /// <summary>
        /// Creates a string representation of a tag. 
        /// </summary>
        /// <param name="tag">The type of the HTML tag that would be generated for every registered resource.</param>
        /// <param name="attributes">The attributes associated with the tag.</param>
        /// <returns>The string representation of a tag.</returns>
        private static string GenerateTag(string tagName, params KeyValuePair<string, string>[] attributes)
        {
            var tag = new TagBuilder(tagName);

            if (attributes != null)
            {
                foreach (var attr in attributes)
                    tag.Attributes[attr.Key] = attr.Value;
            }

            return tag.ToString();
        }

        private const string JsRegisterName = "JsRegister";
        private const string CssRegisterName = "CssRegister";

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

        /// <summary>
        /// This enum represents supported resource types.
        /// </summary>
        private enum ResourceType
        {
            Js,
            Css
        }
    }
}