using System;
using System.Collections;
using System.IO;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    public class DummyResourceResolverNode : ResourceResolverNode
    {
        public Func<PathDefinition, string, bool> CurrentExistsMock;
        public Func<PathDefinition, string, IEnumerable, DateTime, CacheDependency> GetCurrentCacheDependencyMock;
        public Func<PathDefinition, string, Stream> CurrentOpenMock;

        protected override bool CurrentExists(PathDefinition definition, string virtualPath)
        {
            return this.CurrentExistsMock(definition, virtualPath);
        }

        protected override CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return this.GetCurrentCacheDependencyMock(definition, virtualPath, virtualPathDependencies, utcStart);
        }

        protected override Stream CurrentOpen(PathDefinition definition, string virtualPath)
        {
            return this.CurrentOpenMock(definition, virtualPath);
        }
    }
}
