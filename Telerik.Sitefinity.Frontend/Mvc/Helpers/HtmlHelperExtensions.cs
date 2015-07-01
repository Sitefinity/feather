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
        public static System.Web.Mvc.MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName)
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
        public static System.Web.Mvc.MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData)
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
        public static System.Web.Mvc.MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName, object model)
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
        public static System.Web.Mvc.MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                PartialExtensions.RenderPartialInternal(htmlHelper, partialViewName, viewData, model, writer);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        /// <summary>Renders the specified partial view by using the specified HTML helper.</summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">The name of the partial view</param>
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName)
        {
            PartialExtensions.RenderPartialInternal(htmlHelper, partialViewName, htmlHelper.ViewData, null, htmlHelper.ViewContext.Writer);
        }

        /// <summary>Renders the specified partial view, replacing its ViewData property with the specified <see cref="T:System.Web.Mvc.ViewDataDictionary" /> object.</summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="viewData">The view data.</param>
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData)
        {
            PartialExtensions.RenderPartialInternal(htmlHelper, partialViewName, viewData, null, htmlHelper.ViewContext.Writer);
        }

        /// <summary>Renders the specified partial view, passing it a copy of the current <see cref="T:System.Web.Mvc.ViewDataDictionary" /> object, but with the Model property set to the specified model.</summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="model">The model.</param>
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, object model)
        {
            PartialExtensions.RenderPartialInternal(htmlHelper, partialViewName, htmlHelper.ViewData, model, htmlHelper.ViewContext.Writer);
        }

        /// <summary>Renders the specified partial view, replacing the partial view's ViewData property with the specified <see cref="T:System.Web.Mvc.ViewDataDictionary" /> object and setting the Model property of the view data to the specified model.</summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="model">The model for the partial view.</param>
        /// <param name="viewData">The view data for the partial view.</param>
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, object model, ViewDataDictionary viewData)
        {
            PartialExtensions.RenderPartialInternal(htmlHelper, partialViewName, viewData, model, htmlHelper.ViewContext.Writer);
        }

        private static void RenderPartialInternal(HtmlHelper htmlHelper, string partialViewName, ViewDataDictionary viewData, object model, TextWriter writer)
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