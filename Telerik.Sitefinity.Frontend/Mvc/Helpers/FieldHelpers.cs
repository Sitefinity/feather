using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helper methods for rendering of the fields.
    /// </summary>
    public static class FieldHelpers
    {
        /// <summary>
        /// Renders longs the text area field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="dynamicFieldItem">The dynamic field item.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString LongTextAreaField(this HtmlHelper helper, string text, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;

            return ASP.PartialExtensions.Partial(helper, "LongTextAreaField", text);
        }

        /// <summary>
        /// Renders taxonomy field.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="dynamicFieldItem">The dynamic field item.</param>
        /// <param name="classificationId">The classification identifier.</param>
        /// <param name="fieldTitle">The field title.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static System.Web.Mvc.MvcHtmlString TaxonomyField(this HtmlHelper helper, object classificationFieldValue, Guid classificationId, string fieldTitle, string fieldName, string cssClass = "")
        {
            helper.ViewBag.CssClass = cssClass;

            var model = new ClassificationFieldViewModel((IList<Guid>)classificationFieldValue, classificationId, fieldTitle, fieldName);

            if (model.GetTaxonomyType() == Telerik.Sitefinity.Taxonomies.Model.TaxonomyType.Flat)
                return ASP.PartialExtensions.Partial(helper, "FlatTaxonomyField", model);
            else if (model.GetTaxonomyType() == Telerik.Sitefinity.Taxonomies.Model.TaxonomyType.Hierarchical)
                return ASP.PartialExtensions.Partial(helper, "HierarchicalTaxonomyField", model);

            return null;
        }
    }
}
