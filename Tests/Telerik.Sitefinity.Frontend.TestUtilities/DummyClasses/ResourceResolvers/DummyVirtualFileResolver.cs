using System;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    /// <summary>
    /// This is dummy class implements <see cref="Telerik.Sitefinity.Abstractions.VirtualPath.IVirtualFileResolver"/> for testing purposes.
    /// </summary>
    public class DummyVirtualFileResolver : IVirtualFileResolver
    {
        /// <summary>
        /// Simulates that resource always exists on the searched location.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>boolean result</returns>
        public bool Exists(PathDefinition definition, string virtualPath)
        {
            return true;
        }

        /// <inheritdoc />
        public System.Web.Caching.CacheDependency GetCacheDependency(PathDefinition definition, string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream Open(PathDefinition definition, string virtualPaht)
        {
            throw new NotImplementedException();
        }
    }
}
