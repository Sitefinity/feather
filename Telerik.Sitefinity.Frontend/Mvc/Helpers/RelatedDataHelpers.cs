using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helper methods for rendering of the related data fields.
    /// </summary>
    public static class RelatedDataHelpers
    {
        /// <summary>
        /// Renders related data inline list field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItems">The related data items.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedDataInlineListField(this HtmlHelper helper, IList<IDataItem> relatedDataItems, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItems, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.InlineListFieldViewName, model);
        }

        /// <summary>
        /// Renders related data inline single item field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItem">The related data item.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedDataInlineSingleField(this HtmlHelper helper, IDataItem relatedDataItem, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItem, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.InlineSingleFieldViewName, model);
        }

        /// <summary>
        /// Renders related image inline list field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItems">The related data items.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedImageInlineListField(this HtmlHelper helper, IList<IDataItem> relatedDataItems, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItems, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.ImageInlineListFieldViewName, model);
        }

        /// <summary>
        /// Renders single related image inline field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItem">The related data item.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedImageInlineSingleField(this HtmlHelper helper, IDataItem relatedDataItem, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItem, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.ImageInlineSingleFieldViewName, model);
        }

        /// <summary>
        /// Renders related video inline list field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItems">The related data items.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedVideoInlineListField(this HtmlHelper helper, IList<IDataItem> relatedDataItems, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItems, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.VideoInlineListFieldViewName, model);
        }

        /// <summary>
        /// Renders single related video inline field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItem">The related data item.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedVideoInlineSingleField(this HtmlHelper helper, IDataItem relatedDataItem, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItem, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.VideoInlineSingleFieldViewName, model);
        }

        /// <summary>
        /// Renders related document inline list field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItems">The related data items.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedDocumentInlineListField(this HtmlHelper helper, IList<IDataItem> relatedDataItems, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItems, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.DocumentInlineListFieldViewName, model);
        }

        /// <summary>
        /// Renders single related document inline field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItem">The related data item.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedDocumentInlineSingleField(this HtmlHelper helper, IDataItem relatedDataItem, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItem, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.DocumentInlineSingleFieldViewName, model);
        }

        /// <summary>
        /// Renders related page inline list field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItems">The related data items.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedPageInlineListField(this HtmlHelper helper, IList<IDataItem> relatedDataItems, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItems, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.PageInlineListFieldViewName, model);
        }

        /// <summary>
        /// Renders single related page inline field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="relatedDataItem">The related data item.</param>
        /// <param name="identifierField">The identifier field.</param>
        /// <param name="frontendWidgetLabel">The frontend widget label.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString RelatedPageInlineSingleField(this HtmlHelper helper, IDataItem relatedDataItem, string identifierField, string frontendWidgetLabel, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;
            var model = new RelatedDataFieldViewModel(relatedDataItem, frontendWidgetLabel, identifierField);

            return ASP.PartialExtensions.Partial(helper, RelatedDataHelpers.PageInlineSingleFieldViewName, model);
        }

        internal const string InlineListFieldViewName = "RelatedDataInlineListField";
        internal const string InlineSingleFieldViewName = "RelatedDataInlineSingleField";

        internal const string ImageInlineListFieldViewName = "RelatedImageInlineListField";
        internal const string ImageInlineSingleFieldViewName = "RelatedImageInlineSingleField";

        internal const string VideoInlineListFieldViewName = "RelatedVideoInlineListField";
        internal const string VideoInlineSingleFieldViewName = "RelatedVideoInlineSingleField";

        internal const string DocumentInlineListFieldViewName = "RelatedDocumentInlineListField";
        internal const string DocumentInlineSingleFieldViewName = "RelatedDocumentInlineSingleField";

        internal const string PageInlineListFieldViewName = "RelatedPageInlineListField";
        internal const string PageInlineSingleFieldViewName = "RelatedPageInlineSingleField";
    }
}
