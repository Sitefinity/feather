using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class is responsible for locating and resolving of the Layouts.
    /// </summary>
    internal class LayoutVirtualFileResolver : IVirtualFileResolver
    {
        #region Public methods

        /// <summary>
        /// Determines whether a file with the specified virtual path exists.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path to check.</param>
        public virtual bool Exists(PathDefinition definition, string virtualPath)
        {
            virtualPath = this.virtualPathBuilder.RemoveParams(virtualPath);
            virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);

            var layoutVirtualPathBuilder = new LayoutVirtualPathBuilder();
            string viewName = layoutVirtualPathBuilder.GetLayoutName(definition, virtualPath);
            var layoutTemplateBuilder = new LayoutRenderer();

            if (string.IsNullOrEmpty(viewName))
                return false;

            if (virtualPath.StartsWith(string.Format(System.Globalization.CultureInfo.InvariantCulture, "~/{0}", LayoutVirtualFileResolver.ResolverPath), StringComparison.Ordinal))
                return layoutTemplateBuilder.LayoutExists(viewName);
            else
                return false;
        }

        /// <summary>
        /// Opens the the file with the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file to open.</param>
        public virtual Stream Open(PathDefinition definition, string virtualPath)
        {
            var placeholdersOnly = virtualPath.EndsWith(".master", StringComparison.OrdinalIgnoreCase);
            virtualPath = this.virtualPathBuilder.RemoveParams(virtualPath);

            MemoryStream outPutStream = null;
            virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
            var virtualBuilder = new LayoutVirtualPathBuilder();
            var viewName = virtualBuilder.GetLayoutName(definition, virtualPath);
            var layoutHtmlString = this.RenderLayout(viewName, placeholdersOnly);

            if (!string.IsNullOrEmpty(layoutHtmlString))
            {
                var layoutBytes = Encoding.Default.GetBytes(layoutHtmlString.ToCharArray());
                outPutStream = new MemoryStream(layoutBytes);
            }

            return outPutStream;
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
        /// </returns>
        public CacheDependency GetCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            virtualPath = this.virtualPathBuilder.RemoveParams(virtualPath);
            virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);

            var layoutVirtualPathBuilder = new LayoutVirtualPathBuilder();
            string viewName = layoutVirtualPathBuilder.GetLayoutName(definition, virtualPath);
            var layoutTemplateBuilder = new LayoutRenderer();

            if (string.IsNullOrEmpty(viewName))
                return null;

            var viewPath = layoutTemplateBuilder.LayoutViewPath(viewName);
            return HostingEnvironment.VirtualPathProvider.GetCacheDependency(viewPath, virtualPathDependencies, utcStart);
        }

        #endregion

        #region Private Methods

        private string RenderLayout(string pageTemplateName, bool placeholdersOnly)
        {
            var layoutTemplateBuilder = new LayoutRenderer();

            return layoutTemplateBuilder.GetLayoutTemplate(pageTemplateName, placeholdersOnly);
        }

        #endregion

        #region Private fields and constants

        /// <summary>
        /// The resolver path.
        /// </summary>
        public const string ResolverPath = "SfLayouts/";

        /// <summary>
        /// The virtual path builder instance
        /// </summary>
        private VirtualPathBuilder virtualPathBuilder = new VirtualPathBuilder();

        #endregion
    }
}
