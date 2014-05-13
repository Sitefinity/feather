using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    /// <summary>
    /// This is dummy class implemented for testing purposes which implements IVirtualFileResolver.
    /// </summary>
    public class DummyVirtualFileResolver : IVirtualFileResolver
    {
        /// <summary>
        /// Simulates that resource always exists on the searched location.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        public bool Exists(PathDefinition definition, string virtualPath)
        {
            return true;
        }

        /// <inheritdoc />
        public System.Web.Caching.CacheDependency GetCacheDependency(PathDefinition definition, string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public System.IO.Stream Open(PathDefinition definition, string virtualPaht)
        {
            throw new NotImplementedException();
        }
    }
}
