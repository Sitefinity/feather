using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.InlineEditing;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

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
        public static IHtmlString TextField(this HtmlHelper helper, object model, string propName)
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
        public static IHtmlString TextField(this HtmlHelper helper, string propName, string propValue, string fieldType)
        {
            string htmlString;

            if (!SystemManager.IsInlineEditingMode)
            {
                htmlString = propValue;
            }
            else
            {
                htmlString = string.Format(HtmlProcessor.InlineEditingHtmlWrapper, propName, fieldType, propValue);
            }
            
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

        /// <summary>
        /// Renders InlineEditing attributes required for the Inline editing feature.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>The inline editing attributes.</returns>
        public static IHtmlString InlineEditingAttributes(this HtmlHelper htmlHelper, string providerName, string type, Guid id)
        {
            if (!SystemManager.IsInlineEditingMode)
                return htmlHelper.Raw(string.Empty);

            var providerNameEncoded = providerName != null ? htmlHelper.Encode(providerName) : providerName;
            var typeEncoded = htmlHelper.Encode(type);

            if (id == Guid.Empty)
                return htmlHelper.Raw("data-sf-provider='{0}' data-sf-type='{1}'".Arrange(providerNameEncoded, typeEncoded));
            else
                return htmlHelper.Raw("data-sf-provider='{0}' data-sf-type='{1}' data-sf-id='{2}'".Arrange(providerNameEncoded, typeEncoded, id.ToString("D")));
        }

        /// <summary>
        /// Renders InlineEditing attributes for fields required for the Inline editing feature.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns></returns>
        public static IHtmlString InlineEditingFieldAttributes(this HtmlHelper htmlHelper, string fieldName, string fieldType)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");

            if (fieldName == null)
                throw new ArgumentNullException("fieldName");

            if (fieldType == null)
                throw new ArgumentNullException("fieldType");

            if (!SystemManager.IsInlineEditingMode)
                return htmlHelper.Raw(string.Empty);

            var fieldNameEncoded = htmlHelper.Encode(fieldName);
            var fieldTypeEncoded = htmlHelper.Encode(fieldType);

            return htmlHelper.Raw("data-sf-field='{0}' data-sf-ftype='{1}'".Arrange(fieldNameEncoded, fieldTypeEncoded));
        }

        /// <summary>
        /// Returns if the inline editin section should be rendered.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static bool ShouldRenderInlineEditing(this HtmlHelper htmlHelper)
        {
            var shouldRender = false;

            if (!SitefinityContext.IsBackend && ControlExtensions.InlineEditingIsEnabled() && !SystemManager.CurrentHttpContext.Request.IsAjaxRequest())
            {
                const string SiteMapNodeKey = "ServedPageNode";

                if (HttpContext.Current != null && HttpContext.Current.Items != null && HttpContext.Current.Items.Contains(SiteMapNodeKey))
                {
                    var pageSiteNode = HttpContext.Current.Items[SiteMapNodeKey] as PageSiteNode;
                    if (pageSiteNode != null)
                    {
                        var firstPageDataNode = RouteHelper.GetFirstPageDataNode(pageSiteNode, true);
                        if (firstPageDataNode != null && firstPageDataNode.Framework == Pages.Model.PageTemplateFramework.Mvc)
                        {
                            shouldRender = true;
                        }
                    }
                }
            }

            return shouldRender;
        }
    }
}