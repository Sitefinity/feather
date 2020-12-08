using System;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Mvc.Rendering;
using Telerik.Sitefinity.Taxonomies.Model;
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
                url = UrlHelpers.AppendVersion(url);
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

            contentResolvedPath = UrlHelpers.AppendVersion(contentResolvedPath);

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
        /// Gets the URL template used for the paging.
        /// </summary>
        /// <returns>The URL template.</returns>
        public static string GetRedirectPagingUrl()
        {
            string redirectUrl;
            if (UrlParamsMapperBase.UseNamedParametersRouting)
            {
                redirectUrl = "/" + FeatherActionInvoker.PagingNamedParameter + "/{0}";
            }
            else
            {
                redirectUrl = "/{0}";
            }

            return redirectUrl;
        }

        /// <summary>
        /// Gets the URL template used for the paging when filtered by taxonomy.
        /// </summary>
        /// <returns>The URL template.</returns>
        public static string GetRedirectPagingUrl(ITaxon taxonFilter)
        {
            string redirectUrl;
            if (UrlParamsMapperBase.UseNamedParametersRouting)
            {
                redirectUrl = string.Format("/{0}/{1}/{2}", taxonFilter.Taxonomy.Name, taxonFilter.UrlName, FeatherActionInvoker.PagingNamedParameter) + "/{0}";
            }
            else
            {
                redirectUrl = "/" + taxonFilter.UrlName + "/{0}";
            }

            return redirectUrl;
        }

        /// <summary>
        /// Gets the URL template used for the paging when filtered by taxonomy and url evaluation mode.
        /// </summary>
        /// <returns>The URL template.</returns>
        public static string GetRedirectPagingUrl(ITaxon taxonFilter, string[] urlParams, string queryString)
        {
            bool addQueryString = !string.IsNullOrEmpty(queryString);
            string redirectUrl;
            if (UrlParamsMapperBase.UseNamedParametersRouting)
            {
                redirectUrl = string.Format("/{0}/{1}/{2}", taxonFilter.Taxonomy.Name, taxonFilter.UrlName, FeatherActionInvoker.PagingNamedParameter) + "/{0}";
            }
            else
            {
                // 3 because we have the following structure /-in-tags/tags/tagName
                if (urlParams != null && urlParams.Length >= 3)
                {
                    if (taxonFilter is FlatTaxon)
                    {
                        redirectUrl = string.Format("/{0}/{1}/{2}", urlParams[0], urlParams[1], urlParams[2]);
                    }
                    else
                    {
                        if (urlParams[urlParams.Length - 1].Equals(taxonFilter.UrlName))
                        {
                            // url is like /-in-category/categories/cat1/cat2
                            string taxonFilterParams = string.Join("/", urlParams);
                            redirectUrl = "/" + taxonFilterParams;
                        }
                        else
                        {
                            // url is like /-in-category/categories/cat1/cat2/2 where '2' is the page so we want to exclude it from redirect url
                            string taxonFilterParams = string.Join("/", urlParams.Take(urlParams.Length - 1));
                            redirectUrl = "/" + taxonFilterParams;
                        }
                    }

                    // add /{0} at the very end for the page number
                    redirectUrl += "/{0}";
                }
                else
                {
                    addQueryString = false;
                    redirectUrl = "/{0}" + queryString;
                }
            }

            if (addQueryString)
            {
                redirectUrl += queryString;
            }

            return redirectUrl;
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

        internal static string AppendVersion(string contentPath)
        {
            if (contentPath.EndsWith(".js") || contentPath.Contains(".js?"))
            {
                var hash = VirtualPathManager.GetFileHash(contentPath, null);
                if (hash != null)
                {
                    var bytes = Encoding.UTF8.GetBytes(hash);
                    var base64version = Convert.ToBase64String(bytes);
                    contentPath = UrlTransformations.AppendParam(contentPath, VersionQueryParam, base64version);
                }
            }

            return contentPath;
        }

        private const string VersionQueryParam = "v";
    }
}
