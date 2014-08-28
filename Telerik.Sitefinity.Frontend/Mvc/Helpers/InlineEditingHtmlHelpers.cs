using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.InlineEditing;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// HTML helper methods which makes the InlineEiting possible
    /// </summary>
    public static class InlineEditingHtmlHelpers
    {
        /// <summary>
        /// HTML helper which adds the meta data required by InlineEditing.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="model">The object which contains the property.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns></returns>
        public static System.Web.Mvc.MvcHtmlString TextField(this HtmlHelper helper, object model, string propName)
        {
            var htmlProcessor = new HtmlProcessor();
            var htmlString = htmlProcessor.GetStringContent(model, propName);
            return new System.Web.Mvc.MvcHtmlString(htmlString);
        }

        /// <summary>
        /// HTML helper which adds the meta data required by InlineEditing.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="propName">Name of the property.</param>
        /// <param name="propValue">The property value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>Html required for enabling inline editing.</returns>
        public static System.Web.Mvc.MvcHtmlString TextField(this HtmlHelper helper, string propName, string propValue, string fieldType)
        {
            var htmlString = String.Format(HtmlProcessor.InlineEditingHtmlWrapper, propName, fieldType, propValue);
            
            return new System.Web.Mvc.MvcHtmlString(htmlString);
        }
       
        /// <summary>
        /// HTML helper which adds an InlineEditing region. This should be added once at the top of the page, and the whole region will support InlineEditing.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static HtmlRegion InlineEditingRegion(
                                   this HtmlHelper htmlHelper,
                                   string providerName,
                                   string type, 
                                   Guid id)
        {
            var htmlProcessor = new HtmlProcessor();

            return htmlProcessor.CreateInlineEditingRegion(
                htmlHelper.ViewContext.Writer,
                providerName, 
                type, 
                id);
        }
    }
}