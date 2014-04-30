using System;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Extension methods for UrlHelper.
    /// </summary>
    public static class UrlHelpers
    {
        /// <summary>
        /// Resolves URL based on the current widget.
        /// </summary>
        /// <param name="helper">The URL helper.</param>
        /// <param name="contentPath">The content path.</param>
        /// <returns>Resolved URL.</returns>
        /// <exception cref="System.ArgumentNullException">contentPath</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Could not resolve the given URL because RouteData of the current context is null.
        /// or
        /// Could not resolve the given URL because RouteData does not contain \controller\ key.
        /// </exception>
        public static string WidgetContent(this UrlHelper helper, string contentPath)
        {
            var packagesManager = new PackagesManager();

            if (contentPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException("contentPath");
            }

            if (contentPath.StartsWith("~") || contentPath.StartsWith("/") || contentPath.Contains("://"))
            {
                return helper.Content(contentPath);
            }

            if (helper.RequestContext.RouteData == null)
            {
                throw new InvalidOperationException("Could not resolve the given URL because RouteData of the current context is null.");
            }

            string controllerName;
            if (helper.RequestContext.RouteData.Values.ContainsKey("controller"))
            {
                controllerName = (string)helper.RequestContext.RouteData.Values["controller"];
            }
            else
            {
                throw new InvalidOperationException("Could not resolve the given URL because RouteData does not contain \"controller\" key.");
            }

            var controllerType = FrontendManager.ControllerFactory.ResolveControllerType(controllerName);
            var widgetPath = FrontendManager.VirtualPathBuilder.GetVirtualPath(controllerType);
             var packageName = packagesManager.GetCurrentPackage();

            //"widgetName" is a parameter in the route of the Designer. It allows us to have a special fallback logic
            //where we first check for the requested resource in the widget assembly and then fallback to the current controller assembly.
            object widgetName;
            if (helper.RequestContext.RouteData.Values.TryGetValue("widgetName", out widgetName))
            {
                controllerType = FrontendManager.ControllerFactory.ResolveControllerType((string)widgetName);
                var alternatePath = FrontendManager.VirtualPathBuilder.GetVirtualPath(controllerType);
                alternatePath = packagesManager.AppendPackageParam("~/" + alternatePath + contentPath, packageName);
                if (HostingEnvironment.VirtualPathProvider == null || HostingEnvironment.VirtualPathProvider.FileExists(alternatePath))
                {
                    return helper.Content(alternatePath);
                }
            }

            var resolvedPath = packagesManager.AppendPackageParam("~/" + widgetPath + contentPath, packageName);

            //If no resource is found for the current widget virtual path then get URL for Telerik.Sitefinity.Frontend.
            if (HostingEnvironment.VirtualPathProvider == null || HostingEnvironment.VirtualPathProvider.FileExists(resolvedPath))
            {
                return helper.Content(resolvedPath);
            }
            else
            {
                resolvedPath = packagesManager.AppendPackageParam("~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(UrlHelpers).Assembly)
                     + contentPath, packageName);
                return helper.Content(resolvedPath);
            }
        }
    }
}
