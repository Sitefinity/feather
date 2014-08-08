using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class is responsible for locating and resolving of the Layouts.
    /// </summary>
    public class LayoutVirtualFileResolver : IHashedVirtualFileResolver
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

            var virtualPathBuilder = new LayoutVirtualPathBuilder();
            string viewName = virtualPathBuilder.GetLayoutName(definition, virtualPath);
            var layoutTemplateBuilder = new LayoutRenderer();

            if (string.IsNullOrEmpty(viewName))
                return false;

            if (virtualPath.StartsWith(string.Format("~/{0}", LayoutVirtualFileResolver.ResolverPath)))
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
            virtualPath = this.virtualPathBuilder.RemoveParams(virtualPath);

            MemoryStream outPutStream = null;
            virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
            var virtualBuilder = new LayoutVirtualPathBuilder();
            var viewName = virtualBuilder.GetLayoutName(definition, virtualPath);
            var layoutHtmlString = this.RenderLayout(viewName);

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
            return null;
        }

        /// <summary>
        /// Returns a hash of the specified virtual paths.
        /// </summary>
        /// <param name="definition">The file resolver definition.</param>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <param name="virtualPathDependencies">An array of paths to other virtual resources required by the primary virtual resource.</param>
        /// <returns>
        /// A hash of the specified virtual paths.
        /// </returns>
        public string GetFileHash(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies)
        {
            return Guid.NewGuid().ToString();
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the layout.
        /// </summary>
        /// <param name="layoutFileName">Filename of the layout.</param>
        /// <returns>Rendered layout as HTML.</returns>
        private string RenderLayout(string pageTemplateName)
        {
            var layoutTemplateBuilder = new LayoutRenderer();

            return layoutTemplateBuilder.GetLayoutTemplate(pageTemplateName);
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
