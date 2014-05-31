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
            if (contentPath.IsNullOrEmpty())
                throw new ArgumentNullException("contentPath");

            if (contentPath.StartsWith("~") || contentPath.StartsWith("/") || contentPath.Contains("://"))
                return helper.Content(contentPath);

            if (helper.RequestContext.RouteData == null)
                throw new InvalidOperationException("Could not resolve the given URL because RouteData of the current context is null.");

            var packagesManager = new PackageManager();
            var packageName = packagesManager.GetCurrentPackage();

            string contentResolvedPath = string.Empty;
            //"widgetName" is a parameter in the route of the Designer. It allows us to have a special fallback logic
            //where we first check for the requested resource in the widget assembly and then fallback to the current controller assembly.
            object controllerName;
            if (helper.RequestContext.RouteData.Values.TryGetValue("widgetName", out controllerName))
                contentResolvedPath = UrlHelpers.GetResourcePath((string)controllerName, contentPath, PackageManager.PackageUrlParamterName, packageName);

            if (string.IsNullOrEmpty(contentResolvedPath))
            {
                if (helper.RequestContext.RouteData.Values.TryGetValue("controller", out controllerName))
                    contentResolvedPath = UrlHelpers.GetResourcePath((string)controllerName, contentPath, PackageManager.PackageUrlParamterName, packageName);
                else
                    throw new InvalidOperationException("Could not resolve the given URL because RouteData does not contain \"controller\" key.");
            }

            if (string.IsNullOrEmpty(contentResolvedPath))
            {
                var url = "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(UrlHelpers).Assembly) + contentPath;
                contentResolvedPath = UrlTransformations.AppendParam(url, PackageManager.PackageUrlParamterName, packageName);
            }
            return helper.Content(contentResolvedPath);
        }

        /// <summary>
        /// Gets the resource path.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="contentPath">The content path.</param>
        /// <param name="paramaterName">Name of the paramater.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns></returns>
        private static string GetResourcePath(string controllerName, string contentPath, string paramaterName, string packageName)
        {
            var controllerType = FrontendManager.ControllerFactory.ResolveControllerType(controllerName);
            var alternatePath = FrontendManager.VirtualPathBuilder.GetVirtualPath(controllerType);
            var baseUrl = "~/" + alternatePath + contentPath;

            if (HostingEnvironment.VirtualPathProvider == null || HostingEnvironment.VirtualPathProvider.FileExists(baseUrl))
            {
                alternatePath = UrlTransformations.AppendParam(baseUrl, paramaterName, packageName);
                return alternatePath;
            }

            return string.Empty;
        }
    }
}
