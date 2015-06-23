using System;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

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
        public static System.Web.Mvc.MvcHtmlString SfPlaceHolder(this HtmlHelper helper, string containerName = LayoutsHelpers.PlaceHolderDefaultName)
        {
            var htmlString = LayoutsHelpers.ContentPlaceHolderMarkup(containerName);

            return new System.Web.Mvc.MvcHtmlString(htmlString);
        }

        /// <summary>
        /// Html helper which renders all script tags.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="name">The name of the section.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper")]
        public static System.Web.Mvc.MvcHtmlString Section(this HtmlHelper helper, string name)
        {
            if (helper.ViewContext.HttpContext != null)
            {
                SectionRenderer.MarkAvailability(helper.ViewContext.HttpContext.Handler.GetPageHandler(), name);
            }

            return new System.Web.Mvc.MvcHtmlString(LayoutsHelpers.SectionHtml.Arrange(name));
        }

        /// <summary>
        /// Gets the ASPX markup of a content place holder.
        /// </summary>
        /// <param name="placeHolderName">Name of the place holder.</param>
        /// <returns></returns>
        internal static string ContentPlaceHolderMarkup(string placeHolderName = LayoutsHelpers.PlaceHolderDefaultName)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "<asp:contentplaceholder ID='{0}' runat='server' />", placeHolderName);
        }

        /// <summary>
        /// The section tag name.
        /// </summary>
        internal const string SectionTag = "FeatherSection";

        /// <summary>
        /// The section HTML markup.
        /// </summary>
        internal const string SectionHtml = "<" + LayoutsHelpers.SectionTag + " name=\"{0}\"></" + LayoutsHelpers.SectionTag + ">";

        /// <summary>
        /// The content place holder default name.
        /// </summary>
        private const string PlaceHolderDefaultName = "Body";
    }
}
