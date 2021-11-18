﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helper methods for collecting all available views.
    /// </summary>
    public static class ViewSelectorHelpers
    {
        /// <summary>
        /// Gets a collection with the view names which match the given pattern.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="templateNamePattern">The template name pattern.</param>
        /// <returns>Names of the views that match provided pattern.</returns>
        /// <exception cref="System.ArgumentException">Controller cannot be resolved.</exception>
        public static IEnumerable<string> GetViewNames(this HtmlHelper helper, string controllerName, string templateNamePattern)
        {
            var controller = FrontendManager.ControllerFactory.CreateController(helper.ViewContext.RequestContext, controllerName) as Controller;
            if (controller == null)
            {
                throw new ArgumentException("Controller cannot be resolved.");
            }

            if (controller.RouteData != null && controller.RouteData.Values["controller"] as string != null)
            {
                controller.RouteData.Values["controller"] = controllerName;
            }
            var regex = new Regex(templateNamePattern, RegexOptions.IgnoreCase);
            var views = controller.GetViews().Where(view => Regex.IsMatch(view, templateNamePattern)).Select(view => regex.Match(view).Groups["viewName"].Value);

            return views;
        }

        /// <summary>
        /// Gets a collection with the view names which match the given pattern.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="templateNamePattern">The template name pattern.</param>
        /// <returns>Names of the views that match provided pattern.</returns>
        /// <exception cref="System.ArgumentException">Controller cannot be resolved.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper")]
        public static IEnumerable<string> GetViewNames(this HtmlHelper helper, Controller controller, string templateNamePattern)
        {
            return ViewSelectorHelpers.GetViewNames(helper, controller, templateNamePattern, null);
        }

        /// <summary>
        /// Gets a collection with the view names which match the given pattern.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="templateNamePattern">The template name pattern.</param>
        /// <param name="moduleName">The name of dynamic module.</param>
        /// <returns>Names of the views that match provided pattern.</returns>
        /// <exception cref="System.ArgumentException">Controller cannot be resolved.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper")]
        public static IEnumerable<string> GetViewNames(this HtmlHelper helper, Controller controller, string templateNamePattern, string moduleName)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            var regex = new Regex(templateNamePattern, RegexOptions.IgnoreCase);
            var views = controller.GetViews(null, moduleName).Where(view => Regex.IsMatch(view, templateNamePattern)).Select(view => regex.Match(view).Groups["viewName"].Value);

            return views;
        }

        /// <summary>
        /// Splits the camel case.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string SplitCamelCase(this string input)
        {
            var inputWithSpaces = Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
            var capitalizeOnlyFirstLater = inputWithSpaces.First().ToString().ToUpper() + inputWithSpaces.Substring(1).ToLower();

            return capitalizeOnlyFirstLater;
        }
    }
}
