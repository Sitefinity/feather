using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Test.DummyClasses;

namespace Telerik.Sitefinity.Frontend.Test.Resources.Resolvers
{
    [TestClass]
    public class ResourceResolverNodeTest
    {
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency returns aggregated dependency containing the current cache dependency and that of the next node when the current node does not contain the resource on the given virtual path.")]
        public void GetCacheDependency_HasCurrentDependencyCurrentExistsFalse_AggregatesCurrentCacheDependencyWithNext()
        {
            var currentNode = new DummyResourceResolverNode();
            var nextNode = new DummyResourceResolverNode();

            var currentCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            currentNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => currentCacheDependency;
            currentNode.CurrentExistsMock = (pathDefinition, virtualPath) => false;

            var nextCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            nextNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => nextCacheDependency;
            nextNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;

            currentNode.SetNext(nextNode);

            var result = currentNode.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);
            Assert.IsInstanceOfType(result, typeof(AggregateCacheDependency), "Resulting cache dependency is not aggregated.");

            var dependencies = this.GetAggregatedDependencies((AggregateCacheDependency)result);

            Assert.AreEqual(2, dependencies.Count);
            Assert.IsTrue(dependencies.Contains(currentCacheDependency), "The resulting aggregated cache dependency does not contain the dependency of the first node.");
            Assert.IsTrue(dependencies.Contains(nextCacheDependency), "The resulting aggregated cache dependency does not contain the dependency of the second node.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency returns the cache dependency of the next node when the current node has no dependency on the current virtual path and does not contain a resource on it.")]
        public void GetCacheDependency_NoCurrentDependencyCurrentExistsFalse_ReturnsNextCacheDependency()
        {
            var currentNode = new DummyResourceResolverNode();
            var nextNode = new DummyResourceResolverNode();

            currentNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => null;
            currentNode.CurrentExistsMock = (pathDefinition, virtualPath) => false;

            var nextCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            nextNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => nextCacheDependency;
            nextNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;

            currentNode.SetNext(nextNode);

            var result = currentNode.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);
            Assert.IsNotInstanceOfType(result, typeof(AggregateCacheDependency));

            Assert.AreSame(nextCacheDependency, result, "The returned cache dependency is not that of the second node.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether Open method returns the result of Open on the next node if the resource does not exist on the current node.")]
        public void Open_CurrentExistsFalse_ReturnsNextOpenResult()
        {
            var currentNode = new DummyResourceResolverNode();
            var nextNode = new DummyResourceResolverNode();

            currentNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => null;
            var currentStream = new MemoryStream();
            currentNode.CurrentOpenMock = (pathDefinition, virtualPath) => currentStream;
            currentNode.CurrentExistsMock = (pathDefinition, virtualPath) => false;

            var nextCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            var nextStream = new MemoryStream();
            nextNode.CurrentOpenMock = (pathDefinition, virtualPath) => nextStream;
            nextNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;

            currentNode.SetNext(nextNode);

            var result = currentNode.Open(new PathDefinition(), "~/Test");
            Assert.AreSame(nextStream, result, "The returned stream is not that of the second node.");
        }

        private ArrayList GetAggregatedDependencies(AggregateCacheDependency aggregatedDependency)
        {
            var fieldInfo = typeof(AggregateCacheDependency).GetField("_dependencies", BindingFlags.Instance | BindingFlags.NonPublic);
            return (ArrayList)fieldInfo.GetValue(aggregatedDependency);
        }
    }
}
