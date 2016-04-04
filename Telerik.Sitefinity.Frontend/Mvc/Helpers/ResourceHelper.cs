using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
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
        /// <param name="helper">The helper.</param>
        /// <param name="scriptPath">The path to the JavaScript file.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        /// <remarks>
        /// This method uses directly the resource from the <see cref="scriptPath" />.
        /// In case you want to use embedded scripts from Sitefinity check <see cref="ResourceHelper.Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false)" />.
        /// </remarks>
        public static MvcHtmlString Script(this HtmlHelper helper, string scriptPath)
        {
            return ResourceHelper.Script(helper, scriptPath, null, false);
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="scriptPath">The path to the JavaScript file.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        /// <remarks>
        /// This method uses directly the resource from the <see cref="scriptPath" />.
        /// In case you want to use embedded scripts from Sitefinity check <see cref="ResourceHelper.Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false)" />.
        /// </remarks>
        public static MvcHtmlString Script(this HtmlHelper helper, string scriptPath, string sectionName)
        {
            return ResourceHelper.Script(helper, scriptPath, sectionName, true);
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="scriptPath">The path to the JavaScript file.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <param name="throwException">Indicates whether to throw an exception if the specified section does not exist.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        /// <remarks>
        /// This method uses directly the resource from the <see cref="scriptPath" />.
        /// In case you want to use embedded scripts from Sitefinity check <see cref="ResourceHelper.Script(this HtmlHelper helper, ScriptRef scriptReference, bool throwException = false)" />.
        /// </remarks>
        public static MvcHtmlString Script(this HtmlHelper helper, string scriptPath, string sectionName, bool throwException)
        {
            if (ResourceHelper.TryConfigureScriptManager(scriptPath, helper.ViewContext.HttpContext.CurrentHandler))
                return MvcHtmlString.Empty;

            return ResourceHelper.RegisterResource(helper.ViewContext.HttpContext, scriptPath, ResourceType.Js, sectionName, throwException);
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <remarks>
        /// This helper references the same resource existing in Sitefinity.
        /// </remarks>
        /// <param name="helper">The helper.</param>
        /// <param name="scriptReference">The script reference.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString Script(this HtmlHelper helper, ScriptRef scriptReference)
        {
            return ResourceHelper.Script(helper, scriptReference, null, false);
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <remarks>
        /// This helper references the same resource existing in Sitefinity.
        /// </remarks>
        /// <param name="helper">The helper.</param>
        /// <param name="scriptReference">The script reference.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString Script(this HtmlHelper helper, ScriptRef scriptReference, string sectionName)
        {
            return ResourceHelper.Script(helper, scriptReference, sectionName, true);
        }

        /// <summary>
        /// Registers JavaScript reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <remarks>
        /// This helper references the same resource existing in Sitefinity.
        /// </remarks>
        /// <param name="helper">The helper.</param>
        /// <param name="scriptReference">The script reference.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <param name="throwException">Indicates whether to throw an exception if the specified section does not exist.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString Script(this HtmlHelper helper, ScriptRef scriptReference, string sectionName, bool throwException)
        {
            if (ResourceHelper.TryConfigureScriptManager(scriptReference, helper.ViewContext.HttpContext.CurrentHandler))
                return System.Web.Mvc.MvcHtmlString.Empty;

            var references = PageManager.GetScriptReferences(scriptReference).Select(r => new MvcScriptReference(r));

            StringBuilder outputMarkup = new StringBuilder();

            foreach (var script in references)
            {
                var resourceUrl = script.GetResourceUrl();
                outputMarkup.Append(ResourceHelper.RegisterResource(helper.ViewContext.HttpContext, resourceUrl, ResourceType.Js, sectionName, throwException));
            }

            return MvcHtmlString.Create(outputMarkup.ToString());
        }

        /// <summary>
        /// Registers style sheet reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="resourcePath">The path to the CSS file.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString StyleSheet(this HtmlHelper helper, string resourcePath)
        {
            return ResourceHelper.StyleSheet(helper, resourcePath, null, false);
        }

        /// <summary>
        /// Registers style sheet reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="resourcePath">The path to the CSS file.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString StyleSheet(this HtmlHelper helper, string resourcePath, string sectionName)
        {
            return ResourceHelper.StyleSheet(helper, resourcePath, sectionName, true);
        }

        /// <summary>
        /// Registers style sheet reference and ensures that it loads maximum once for a page.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="resourcePath">The path to the CSS file.</param>
        /// <param name="sectionName">The name of the section that will render this script. If null it will render on the same place of the page</param>
        /// <param name="throwException">Indicates whether to throw an exception if the specified section does not exist.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString StyleSheet(this HtmlHelper helper, string resourcePath, string sectionName, bool throwException)
        {
            return ResourceHelper.RegisterResource(helper.ViewContext.HttpContext, resourcePath, ResourceType.Css, sectionName, throwException);
        }

        /// <summary>
        /// Adds script references required by the Sitefinity's QueryBuilder component.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString QueryBuilderScriptReferences(this HtmlHelper helper)
        {
            var result = new StringBuilder();
            var urlHelper = new UrlHelper(helper.ViewContext.HttpContext.Request.RequestContext);
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource("Telerik.Sitefinity.Web.UI.QueryBuilder", "Telerik.Sitefinity.Web.UI.QueryBuilder.Scripts.QueryData.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource("Telerik.Sitefinity.Web.UI.QueryBuilder", "Telerik.Sitefinity.Web.UI.QueryBuilder.Scripts.QueryDataItem.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource("Telerik.Sitefinity.Web.UI.QueryBuilder", "Telerik.Sitefinity.Web.UI.QueryBuilder.Scripts.QueryItem.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource("Telerik.Sitefinity.Web.UI.QueryBuilder", "Telerik.Sitefinity.Web.UI.QueryBuilder.Scripts.QueryGroup.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource("Telerik.Sitefinity.Web.UI.QueryBuilder", "Telerik.Sitefinity.Web.UI.QueryBuilder.Scripts.QueryBuilder.js")).ToHtmlString());

            return MvcHtmlString.Create(result.ToString());
        }

        /// <summary>
        /// Adds script references required by the Sitefinity's CodeMirror component.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns>
        /// MvcHtmlString
        /// </returns>
        public static MvcHtmlString CodeMirrorScriptReferences(this HtmlHelper helper)
        {
            var result = new StringBuilder();
            var urlHelper = new UrlHelper(helper.ViewContext.HttpContext.Request.RequestContext);
            result.Append(ResourceHelper.StyleSheet(helper, urlHelper.EmbeddedResource(typeof(Telerik.Sitefinity.Resources.Reference).FullName, "Telerik.Sitefinity.Resources.Scripts.CodeMirror.codemirror.css")).ToHtmlString());
            result.Append(ResourceHelper.StyleSheet(helper, urlHelper.EmbeddedResource(typeof(Telerik.Sitefinity.Resources.Reference).FullName, "Telerik.Sitefinity.Resources.Scripts.CodeMirror.Theme.default.css")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource(typeof(Telerik.Sitefinity.Resources.Reference).FullName, "Telerik.Sitefinity.Resources.Scripts.CodeMirror.codemirror.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource(typeof(Telerik.Sitefinity.Resources.Reference).FullName, "Telerik.Sitefinity.Resources.Scripts.CodeMirror.Mode.htmlmixed.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource(typeof(Telerik.Sitefinity.Resources.Reference).FullName, "Telerik.Sitefinity.Resources.Scripts.CodeMirror.Mode.xml.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource(typeof(Telerik.Sitefinity.Resources.Reference).FullName, "Telerik.Sitefinity.Resources.Scripts.CodeMirror.Mode.css.js")).ToHtmlString());
            result.Append(ResourceHelper.Script(helper, urlHelper.EmbeddedResource(typeof(Telerik.Sitefinity.Resources.Reference).FullName, "Telerik.Sitefinity.Resources.Scripts.CodeMirror.Mode.javascript.js")).ToHtmlString());

            return MvcHtmlString.Create(result.ToString());
        }

        /// <summary>
        /// Renders the lang attribute.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public static MvcHtmlString RenderLangAttribute(this HtmlHelper helper)
        {
            return RenderLangAttribute(helper, CultureInfo.CurrentUICulture.Name);
        }

        /// <summary>
        /// Renders the lang attribute.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static MvcHtmlString RenderLangAttribute(this HtmlHelper helper, string culture)
        {
            string attributeString = helper.FormatValue(culture, "lang=\"{0}\"");

            return new MvcHtmlString(attributeString);
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
                var page = HttpContext.Current.Handler.GetPageHandler() ?? new PageProxy(null);

                resourceUrl = page.ClientScript.GetWebResourceUrl(
                    TypeResolutionService.ResolveType("Telerik.Sitefinity.Resources.Reference"),
                    scriptConfig.Name);
            }

            return resourceUrl;
        }

        /// <summary>
        /// Renders all scripts.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sectionName">The section name.</param>
        /// <returns></returns>
        internal static string RenderAllScripts(HttpContextBase context, string sectionName)
        {
            var scriptRegister = new ResourceRegister(ResourceHelper.JsRegisterName, context);
            var scriptMarkup = ResourceHelper.BuildHtmlResourcesMarkup(scriptRegister, sectionName, ResourceType.Js);

            return scriptMarkup;
        }

        /// <summary>
        /// Renders all stylesheets.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sectionName">The section name.</param>
        /// <returns></returns>
        internal static string RenderAllStylesheets(HttpContextBase context, string sectionName)
        {
            var stylesheetRegister = new ResourceRegister(ResourceHelper.CssRegisterName, context);
            var stylesheetMarkup = ResourceHelper.BuildHtmlResourcesMarkup(stylesheetRegister, sectionName, ResourceType.Css);

            return stylesheetMarkup;
        }

        private static MvcHtmlString RegisterResource(HttpContextBase httpContext, string resourcePath, ResourceType resourceType, string sectionName, bool throwException)
        {
            throwException = throwException && httpContext.CurrentHandler != null;

            var registerName = string.Empty;
            if (resourceType == ResourceType.Js)
                registerName = ResourceHelper.JsRegisterName;
            else if (resourceType == ResourceType.Css)
                registerName = ResourceHelper.CssRegisterName;

            var register = new ResourceRegister(registerName, httpContext);

            MvcHtmlString result = MvcHtmlString.Empty;

            // No section name renders the script inline if it hasn't been rendered
            if (string.IsNullOrEmpty(sectionName) || !SectionRenderer.IsAvailable(httpContext.Handler.GetPageHandler(), sectionName))
            {
                if (!register.IsRegistered(resourcePath, sectionName: null))
                {
                    result = MvcHtmlString.Create(ResourceHelper.BuildSingleResourceMarkup(resourcePath, resourceType));
                }
            }

            // Register the resource even if it had to be rendered inline (avoid repetitions).
            register.Register(resourcePath, sectionName, throwException);

            return result;
        }

        /// <summary>
        /// Tries the configure script manager.
        /// </summary>
        /// <param name="scriptReference">The script reference.</param>
        /// <param name="httpHandler">The httpHandler.</param>
        /// <returns></returns>
        private static bool TryConfigureScriptManager(ScriptRef scriptReference, IHttpHandler httpHandler)
        {
            var page = ResourceHelper.GetTopMostPageFromHandler(httpHandler);

            if (page != null)
            {
                var scriptManager = PageManager.ConfigureScriptManager(page, scriptReference, false);

                if (scriptManager != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries the configure script manager.
        /// </summary>
        /// <param name="scriptReference">The script reference.</param>
        /// <param name="httpHandler">The httpHandler.</param>
        /// <returns></returns>
        private static bool TryConfigureScriptManager(string scriptReference, IHttpHandler httpHandler)
        {
            var page = ResourceHelper.GetTopMostPageFromHandler(httpHandler);

            if (page != null)
            {
                var scriptManager = ScriptManager.GetCurrent(page);

                if (scriptManager != null)
                {
                    scriptManager.Scripts.Add(new ScriptReference(scriptReference));
                    return true;
                }
            }

            return false;
        }

        private static Page GetTopMostPageFromHandler(IHttpHandler httpHandler)
        {
            var page = httpHandler.GetPageHandler();

            if (page != null)
            {
                var topMostPage = page;

                while (topMostPage.PreviousPage != null)
                {
                    topMostPage = topMostPage.PreviousPage;
                }

                return topMostPage;
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sectionName")]
        private static string BuildHtmlResourcesMarkup(ResourceRegister resourceRegister, string sectionName, ResourceType resourceType)
        {
            StringBuilder output = new StringBuilder();

            foreach (var resource in resourceRegister.GetResourcesForSection(sectionName))
            {
                if (!resourceRegister.IsRendered(resource))
                {
                    output.Append(ResourceHelper.BuildSingleResourceMarkup(resource, resourceType));
                    resourceRegister.MarkAsRendered(resource);
                }
            }

            return output.ToString();
        }

        private static string BuildSingleResourceMarkup(string resourceKey, ResourceType resourceType)
        {
            string result;

            if (resourceType == ResourceType.Js)
                result = ResourceHelper.BuildScriptMarkup(resourceKey);
            else if (resourceType == ResourceType.Css)
                result = ResourceHelper.BuildStyleSheetMarkup(resourceKey);
            else
                result = string.Empty;

            return result;
        }

        private static string BuildScriptMarkup(string resourceKey)
        {
            var tag = new TagBuilder("script");

            tag.Attributes["src"] = resourceKey;
            tag.Attributes["type"] = "text/javascript";

            return tag.ToString(TagRenderMode.Normal);
        }

        private static string BuildStyleSheetMarkup(string resourceKey)
        {
            var tag = new TagBuilder("link");

            tag.Attributes["href"] = resourceKey;
            tag.Attributes["rel"] = "stylesheet";
            tag.Attributes["type"] = "text/css";

            return tag.ToString(TagRenderMode.SelfClosing);
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
                var scriptManager = new ScriptManager();

                try
                {
                    return this.GetUrl(scriptManager, zip: false);
                }
                catch (NullReferenceException ex)
                {
                    ////This is because ScriptReference.ClientUrlResolver is null as the class in to initialize on the page.
                    if (this.IsGetUrlFromPathException(ex))
                    {
                        return scriptManager.ResolveClientUrl(this.Path);
                    }

                    throw;
                }
            }

            private bool IsGetUrlFromPathException(Exception ex)
            {
                return ex.TargetSite.Name == "GetUrlFromPath" && ex.TargetSite.DeclaringType.FullName == "System.Web.UI.ScriptReference";
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