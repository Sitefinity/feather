using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class builds the layout master page by using the Mvc view engines.
    /// </summary>
    internal class LayoutRenderer
    {
        #region Public methods

        /// <summary>
        /// Creates a controller instance and sets its ControllerContext depending on the current Http context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routeData">The route data.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Can't create Controller Context if no active HttpContext instance is available.</exception>
        public virtual Controller CreateController(RouteData routeData = null)
        {
            // create a disconnected controller instance
            var controller = new GenericController();
            HttpContextBase context = null;

            // get context wrapper from HttpContext if available
            if (SystemManager.CurrentHttpContext != null)
                context = SystemManager.CurrentHttpContext;
            else
                throw new InvalidOperationException("Can not create ControllerContext if no active HttpContext instance is available.");

            if (routeData == null)
                routeData = new RouteData();

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") &&
                !routeData.Values.ContainsKey("Controller"))
            {
                routeData.Values.Add("controller", "Generic");
            }

            controller.UpdateViewEnginesCollection(() => this.GetPathTransformations());

            // here we create the context for the controller passing the just created controller the httpcontext and the route data that we built above
            controller.ControllerContext = new ControllerContext(context, routeData, controller);

            return controller;
        }

        /// <summary>
        /// Renders the view to string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="placeholdersOnly"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException">View cannot be found.</exception>
        public virtual string RenderViewToString(ControllerContext context, string viewPath, bool placeholdersOnly = false)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (viewPath == null)
                throw new ArgumentNullException("viewPath");
            
            string result = null;
            IView view = null;

            // Obtaining the view engine result
            var viewEngineResult = this.GetViewEngineResult(context, viewPath, isPartial: false);

            if (viewEngineResult != null)
                view = viewEngineResult.View;

            if (view != null)
            {
                // assigning the model so it can be available to the view 
                context.Controller.ViewData.Model = null;

                var builtView = view as BuildManagerCompiledView;
                using (var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
                {
                    if (placeholdersOnly && builtView != null)
                    {
                        this.RenderContentPlaceHolders(builtView.ViewPath, writer);
                    }
                    else
                    {
                        var viewConext = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, writer);
                        view.Render(viewConext, writer);
                    }

                    // the view must be released
                    viewEngineResult.ViewEngine.ReleaseView(context, view);
                    result = writer.ToString();
                }

                // Add cache dependency on the virtual file that is used for rendering the view.
                var httpContext = SystemManager.CurrentHttpContext;
                if (httpContext != null && builtView != null)
                {
                    var virtualPathDependency = HostingEnvironment.VirtualPathProvider != null ?
                        HostingEnvironment.VirtualPathProvider.GetCacheDependency(builtView.ViewPath, null, DateTime.UtcNow) : null;
                    if (virtualPathDependency != null)
                    {
                        httpContext.Response.AddCacheDependency(virtualPathDependency);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the view engine result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="isPartial">if set to <c>true</c> method returns only partial views.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException">View cannot be found.</exception>
        public virtual ViewEngineResult GetViewEngineResult(ControllerContext context, string viewPath, bool isPartial = false)
        {
            var controller = (Controller)context.Controller;
            ViewEngineResult viewEngineResult;

            if (isPartial)
                viewEngineResult = controller.ViewEngineCollection.FindPartialView(context, viewPath);
            else
                viewEngineResult = controller.ViewEngineCollection.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            return viewEngineResult;
        }

        #endregion

        #region ILayoutTemplateBuilder implementation

        /// <summary>
        /// Gets the layout template.
        /// </summary>
        /// <param name="templateName">The name of the layout template to render.</param>
        /// <param name="placeholdersOnly">When true the method returns a master page containing only content placeholders.</param>
        /// <returns></returns>
        public virtual string GetLayoutTemplate(string templateName, bool placeholdersOnly = false)
        {
            var genericController = this.CreateController();
            var layoutHtmlString = this.RenderViewToString(genericController.ControllerContext, templateName, placeholdersOnly);

            if (!layoutHtmlString.IsNullOrEmpty() && !placeholdersOnly)
            {
                var htmlProcessor = new MasterPageBuilder();
                layoutHtmlString = htmlProcessor.ProcessLayoutString(layoutHtmlString);
                layoutHtmlString = htmlProcessor.AddMasterPageDirectives(layoutHtmlString);
            }

            return layoutHtmlString;
        }

        /// <summary>
        /// Determines whether a layout with specified name exists.
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="partial">if set to <c>true</c> the result view should be partial.</param>
        /// <returns></returns>
        public bool LayoutExists(string templateName, bool partial = false)
        {
            var genericController = this.CreateController();
            var viewEngineResult = this.GetViewEngineResult(genericController.ControllerContext, templateName, partial);
            bool result = viewEngineResult != null && viewEngineResult.View != null;

            if (result)
                viewEngineResult.ViewEngine.ReleaseView(genericController.ControllerContext, viewEngineResult.View);

            return result;
        }

        /// <summary>
        /// Gets the file name of the layout.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <returns>File name of the layout.</returns>
        public string LayoutViewPath(string templateName)
        {
            string result = null;

            var genericController = this.CreateController();
            var viewEngineResult = this.GetViewEngineResult(genericController.ControllerContext, templateName, isPartial: false);

            if (viewEngineResult != null && viewEngineResult.View != null)
            {
                var builtView = viewEngineResult.View as BuildManagerCompiledView;
                if (builtView != null)
                {
                    result = builtView.ViewPath;
                }

                viewEngineResult.ViewEngine.ReleaseView(genericController.ControllerContext, viewEngineResult.View);
            }

            return result;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the path transformations.
        /// </summary>
        /// <returns></returns>
        private IList<Func<string, string>> GetPathTransformations()
        {
            var packagesManager = new PackageManager();

            var currentPackage = packagesManager.GetCurrentPackage();
            var pathTransformations = new List<Func<string, string>>(1);
            var baseVirtualPath = FrontendManager.VirtualPathBuilder.GetVirtualPath(this.GetType().Assembly);

            pathTransformations.Add(path =>
                {
                    // {1} is the ControllerName argument in VirtualPathProviderViewEngines
                    var result = path
                                    .Replace("{1}", "Layouts")
                                    .Replace("~/", "~/{0}Mvc/".Arrange(baseVirtualPath));

                    result = (new VirtualPathBuilder()).AddParams(result, currentPackage);

                    return result;
                });

            return pathTransformations;
        }

        private void RenderContentPlaceHolders(string layoutPath, StringWriter writer)
        {
            writer.WriteLine("<%@ Master Language=\"C#\" %>");
            writer.WriteLine("<html><head runat=\"server\"></head><body>");

            string layoutText;
            using (var layoutFile = new StreamReader(HostingEnvironment.VirtualPathProvider.GetFile(layoutPath).Open()))
            {
                layoutText = layoutFile.ReadToEnd();
            }

            var matches = new Regex(@"@Html\.SfPlaceHolder(?:\(\s*\""(?<placeHolder>\w*)\""\s*\)|(?<placeHolder>\(\)))").Matches(layoutText);
            foreach (Match match in matches)
            {
                var group = match.Groups["placeHolder"];
                if (group.Success)
                {
                    foreach (Capture capture in group.Captures)
                    {
                        var placeHolderName = capture.Value != "()" ? capture.Value : null;
                        writer.Write(LayoutsHelpers.ContentPlaceHolderMarkup(placeHolderName));
                    }
                }
            }

            writer.Write("</body></html>");
        }

        #endregion
    }
}
