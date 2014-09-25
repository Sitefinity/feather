using System;
using System.Collections;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    /// <summary>
    /// This class represents dummy implementation of <see cref="Telerik.Sitefinity.Frontend.Resources.Resolvers.ResourceResolverStrategy" /> in order to test whether its methods are being invoked properly.
    /// </summary>
    internal class DummyResolverStrategy : ResourceResolverStrategy
    {
        /// <summary>
        /// A function that will be called through <see cref="Exists"/> method.
        /// </summary>
        public Func<PathDefinition, string, bool> ExistsMock { get; set; }

        /// <summary>
        /// A function that will be called through <see cref="GetCacheDependency"/> method.
        /// </summary>
        public Func<PathDefinition, string, IEnumerable, DateTime, CacheDependency> GetCacheDependencyMock { get; set; }

        /// <inheritdoc />
        public override bool Exists(PathDefinition definition, string virtualPath)
        {
            if (this.ExistsMock != null)
            {
                return this.ExistsMock(definition, virtualPath);
            }
            else
            {
                return base.Exists(definition, virtualPath);
            }
        }

        /// <inheritdoc />
        public override CacheDependency GetCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (this.GetCacheDependencyMock != null)
            {
                return this.GetCacheDependencyMock(definition, virtualPath, virtualPathDependencies, utcStart);
            }
            else
            {
                return base.GetCacheDependency(definition, virtualPath, virtualPathDependencies, utcStart);
            }
        }
    }
}
