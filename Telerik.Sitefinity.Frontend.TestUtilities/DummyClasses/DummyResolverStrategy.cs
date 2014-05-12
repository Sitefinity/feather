using System;
using System.Collections;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    public class DummyResolverStrategy : ResourceResolverStrategy
    {
        public Func<PathDefinition, string, bool> ExistsMock;
        public Func<PathDefinition, string, IEnumerable, DateTime, CacheDependency> GetCacheDependencyMock;

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
