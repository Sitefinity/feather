using System;
using System.Globalization;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Rendering;
using Telerik.Sitefinity.Utilities.TypeConverters;

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

            var packagesManager = new PackageManager();
            var packageName = packagesManager.GetCurrentPackage();

            if (contentPath.StartsWith("~", StringComparison.Ordinal) || contentPath.StartsWith("/", StringComparison.Ordinal) || contentPath.Contains("://"))
            {
                var url = UrlTransformations.AppendParam(contentPath, PackageManager.PackageUrlParameterName, packageName);
                return helper.Content(url);
            }

            if (helper.RequestContext.RouteData == null)
                throw new InvalidOperationException("Could not resolve the given URL because RouteData of the current context is null.");

            var contentResolvedPath = string.Empty;
            object controllerName;

            // "widgetName" is a parameter in the route of the Designer. It allows us to have a special fallback logic
            // where we first check for the requested resource in the widget assembly and then fallback to the current controller assembly.
            if (helper.RequestContext.RouteData.Values.TryGetValue("widgetName", out controllerName))
                contentResolvedPath = UrlHelpers.GetResourcePath((string)controllerName, contentPath, PackageManager.PackageUrlParameterName, packageName);

            if (string.IsNullOrEmpty(contentResolvedPath))
            {
                if (helper.RequestContext.RouteData.Values.TryGetValue("controller", out controllerName))
                    contentResolvedPath = UrlHelpers.GetResourcePath((string)controllerName, contentPath, PackageManager.PackageUrlParameterName, packageName);
                else
                    throw new InvalidOperationException("Could not resolve the given URL because RouteData does not contain \"controller\" key.");
            }

            if (string.IsNullOrEmpty(contentResolvedPath))
            {
                var url = "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(UrlHelpers).Assembly) + contentPath;
                contentResolvedPath = UrlTransformations.AppendParam(url, PackageManager.PackageUrlParameterName, packageName);
            }

            return helper.Content(contentResolvedPath);
        }

        /// <summary>
        /// Resolves URL based on the current widget.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="contentPath">The content path.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">contentPath</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public static string WidgetContent(this UrlHelper helper, string contentPath, string assemblyName)
        {
            if (contentPath.IsNullOrEmpty())
                throw new ArgumentNullException("contentPath");

            if (string.IsNullOrEmpty(assemblyName))
                return UrlHelpers.WidgetContent(helper, contentPath);

            var packagesManager = new PackageManager();
            var packageName = packagesManager.GetCurrentPackage();

            if (contentPath.StartsWith("~", StringComparison.Ordinal) || contentPath.StartsWith("/", StringComparison.Ordinal) || contentPath.Contains("://"))
            {
                var url = UrlTransformations.AppendParam(contentPath, PackageManager.PackageUrlParameterName, packageName);
                return helper.Content(url);
            }

            var resourceUrl = string.Format("~/{0}/{1}", FrontendManager.VirtualPathBuilder.GetVirtualPath(assemblyName), contentPath);
            var contentResolvedPath = UrlTransformations.AppendParam(resourceUrl, PackageManager.PackageUrlParameterName, packageName);

            return helper.Content(contentResolvedPath);
        }

        /// <summary>
        /// Gets the URL of an embedded resource.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="type">A type from the assembly that embeds the resource.</param>
        /// <param name="path">The resource path.</param>
        /// <returns>The resource URL.</returns>
        public static string EmbeddedResource(this UrlHelper helper, string type, string path)
        {
            var page = helper.RequestContext.HttpContext.Handler.GetPageHandler() ?? new PageProxy(null);
            return page.ClientScript.GetWebResourceUrl(TypeResolutionService.ResolveType(type), path);
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
            if (controllerType != null)
            {
                var alternatePath = FrontendManager.VirtualPathBuilder.GetVirtualPath(controllerType);
                var baseUrl = "~/" + alternatePath + contentPath;

                if (HostingEnvironment.VirtualPathProvider == null || HostingEnvironment.VirtualPathProvider.FileExists(baseUrl))
                {
                    alternatePath = UrlTransformations.AppendParam(baseUrl, paramaterName, packageName);
                    return alternatePath;
                }
            }

            return string.Empty;
        }
    }
}
