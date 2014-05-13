using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;

namespace Telerik.Sitefinity.Frontend.Test.Resources.Resolvers
{
    [TestClass]
    public class ResourceResolverStrategyTest
    {
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether Open throws InvalidOperationException when no nodes are present.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Open_NoNodes_ThrowsInvalidOperationException()
        {
            var strategy = new ResourceResolverStrategy();
            strategy.SetFirst(null);
            strategy.Open(new PathDefinition(), "~/Test");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency calls GetCacheDependency method of its first node and returns its result.")]
        public void GetCacheDependency_ReturnsFirstNodesCacheDependency()
        {
            var strategy = new ResourceResolverStrategy();

            var nodeCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            var node = new DummyResourceResolverNode();
            strategy.SetFirst(node);

            node.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcStart) => nodeCacheDependency;
            var resultCacheDependency = strategy.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);

            Assert.AreSame(nodeCacheDependency, resultCacheDependency, "GetCacheDependency did not return the instance that is provided by the node in the strategy.");
        }
    }
}
