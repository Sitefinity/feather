// It is important to keep the namespace. This way these extensions take precedence over the default System.Web.Mvc.Html.PartialExtensions.
namespace ASP
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web.Mvc;

    /// <summary>
    /// <see cref="HtmlHelper"/> extensions methods for rendering partial views.
    /// </summary>
    public static class PartialExtensions
    {
        /// <summary>
        /// Renders the partial view with the specified name.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">Name of the partial view.</param>
        /// <returns>Rendered partial view.</returns>
        public static MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName)
        {
            return PartialExtensions.Partial(htmlHelper, partialViewName, null, htmlHelper.ViewData);
        }

        /// <summary>
        /// Renders the partial view with the specified name.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">Name of the partial view.</param>
        /// <param name="viewData">The view data.</param>
        /// <returns>
        /// Rendered partial view.
        /// </returns>
        public static MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData)
        {
            return PartialExtensions.Partial(htmlHelper, partialViewName, null, viewData);
        }

        /// <summary>
        /// Renders the partial view with the specified name.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">Name of the partial view.</param>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Rendered partial view.
        /// </returns>
        public static MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName, object model)
        {
            return PartialExtensions.Partial(htmlHelper, partialViewName, model, htmlHelper.ViewData);
        }

        /// <summary>
        /// Renders the partial view with the specified name.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">Name of the partial view.</param>
        /// <param name="model">The model.</param>
        /// <param name="viewData">The view data.</param>
        /// <returns>
        /// Rendered partial view.
        /// </returns>
        public static MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                PartialExtensions.RenderPartial(htmlHelper, partialViewName, viewData, model, writer);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        private static void RenderPartial(HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData, object model, TextWriter writer)
        {
            ViewDataDictionary newViewData;
            if (model == null)
            {
                newViewData = viewData == null ? new ViewDataDictionary(htmlHelper.ViewData) : new ViewDataDictionary(viewData);
            }
            else
            {
                newViewData = viewData == null ? new ViewDataDictionary(model) : new ViewDataDictionary(viewData) { Model = model };
            }

            var controller = htmlHelper.ViewContext.Controller as Controller;
            var viewEngineCollection = controller != null ? controller.ViewEngineCollection : ViewEngines.Engines;
            var newViewContext = new ViewContext(htmlHelper.ViewContext, htmlHelper.ViewContext.View, newViewData, htmlHelper.ViewContext.TempData, writer);
            var result = viewEngineCollection.FindPartialView(newViewContext, partialViewName);
            if (result.View != null)
            {
                result.View.Render(newViewContext, writer);
            }
            else
            {
                var locationsText = new StringBuilder();
                foreach (string location in result.SearchedLocations)
                {
                    locationsText.AppendLine();
                    locationsText.Append(location);
                }

                throw new InvalidOperationException("The partial view '{0}' was not found or no view engine supports the searched locations. The following locations were searched: {1}".Arrange(partialViewName, locationsText));
            }
        }
    }
}