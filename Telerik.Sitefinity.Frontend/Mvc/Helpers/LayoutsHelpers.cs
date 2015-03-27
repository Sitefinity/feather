using System;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helper methods for work with Sitefinity layout templates.
    /// </summary>
    public static class LayoutsHelpers
    {
        /// <summary>
        /// HTML helper which adds the required content placeholder.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        public static System.Web.Mvc.MvcHtmlString SfPlaceHolder(this HtmlHelper helper, string containerName = "Body")
        {
            var htmlString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<asp:contentplaceholder ID='{0}' runat='server' />", containerName);

            return new System.Web.Mvc.MvcHtmlString(htmlString);
        }

        /// <summary>
        /// Html helper which renders all script tags.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper")]
        public static System.Web.Mvc.MvcHtmlString SfScriptRenderer(this HtmlHelper helper)
        {
            return new System.Web.Mvc.MvcHtmlString(LayoutsHelpers.ScriptRendererHtml);
        }

        /// <summary>
        /// Html helper which renders all stylesheet tags.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper")]
        public static System.Web.Mvc.MvcHtmlString SfStyleSheetRenderer(this HtmlHelper helper)
        {
            var htmlString = "<feather-stylesheet-renderer></feather-stylesheet-renderer>";

            return new System.Web.Mvc.MvcHtmlString(htmlString);
        }

        /// <summary>
        /// The script renderer tag name.
        /// </summary>
        internal const string ScriptRendererTag = "FeatherScriptRenderer";

        /// <summary>
        /// The script renderer HTML markup.
        /// </summary>
        internal const string ScriptRendererHtml = "<" + LayoutsHelpers.ScriptRendererTag + "></" + LayoutsHelpers.ScriptRendererTag + ">";
    }
}
