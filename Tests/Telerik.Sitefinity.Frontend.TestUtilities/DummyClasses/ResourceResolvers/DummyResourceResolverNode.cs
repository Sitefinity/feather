using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    /// <summary>
    /// This class fakes the <see cref="Telerik.Sitefinity.Frontend.Resources.Resolvers.ResourceResolverNode"/> for test purposes.
    /// </summary>
    internal class DummyResourceResolverNode : ResourceResolverNode
    {
        /// <inheritdoc />
        protected override bool CurrentExists(PathDefinition definition, string virtualPath)
        {
            return this.CurrentExistsMock(definition, virtualPath);
        }

        /// <inheritdoc />
        protected override CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return this.GetCurrentCacheDependencyMock(definition, virtualPath, virtualPathDependencies, utcStart);
        }

        /// <inheritdoc />
        protected override Stream CurrentOpen(PathDefinition definition, string virtualPath)
        {
            return this.CurrentOpenMock(definition, virtualPath);
        }

        /// <inheritdoc />
        protected override IEnumerable<string> GetCurrentFiles(PathDefinition definition, string path)
        {
            return null;
        }

        /// <summary>
        /// A function that will be called through <see cref="CurrentExists"/> method.
        /// </summary>
        public Func<PathDefinition, string, bool> CurrentExistsMock { get; set; }

        /// <summary>
        /// A function that will be called through <see cref="GetCurrentCacheDependency"/> method.
        /// </summary>
        public Func<PathDefinition, string, IEnumerable, DateTime, CacheDependency> GetCurrentCacheDependencyMock { get; set; }

        /// <summary>
        /// A function that will be called through <see cref="CurrentOpen"/> method.
        /// </summary>
        public Func<PathDefinition, string, Stream> CurrentOpenMock { get; set; }
    }
}
