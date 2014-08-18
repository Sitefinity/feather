using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Web.Caching;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources.Resolvers
{
    /// <summary>
    /// Ensures that the ResourceResolverNode class is working correctly.
    /// </summary>
    [TestClass]
    public class ResourceResolverNodeTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency returns aggregated dependency containing the current cache dependency and that of the next node when the current node does not contain the resource on the given virtual path.")]
        public void GetCacheDependency_HasCurrentDependencyCurrentExistsFalse_AggregatesCurrentCacheDependencyWithNext()
        {
            // Arrange
            var currentNode = new DummyResourceResolverNode();

            var currentCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            currentNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => currentCacheDependency;
            currentNode.CurrentExistsMock = (pathDefinition, virtualPath) => false;

            var nextCacheDependency = this.SetNextNode(currentNode);

            // Act
            var result = currentNode.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);

            // Assert
            Assert.IsInstanceOfType(result, typeof(AggregateCacheDependency), "Resulting cache dependency is not aggregated.");

            var dependencies = this.GetAggregatedDependencies((AggregateCacheDependency)result);

            Assert.AreEqual(2, dependencies.Count, "The dependencies count is not correct.");
            Assert.IsTrue(dependencies.Contains(currentCacheDependency), "The resulting aggregated cache dependency does not contain the dependency of the first node.");
            Assert.IsTrue(dependencies.Contains(nextCacheDependency), "The resulting aggregated cache dependency does not contain the dependency of the second node.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency returns the cache dependency of the next node when the current node has no dependency on the current virtual path and does not contain a resource on it.")]
        public void GetCacheDependency_NoCurrentDependencyCurrentExistsFalse_ReturnsNextCacheDependency()
        {
            // Arrange
            var currentNode = new DummyResourceResolverNode();

            currentNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => null;
            currentNode.CurrentExistsMock = (pathDefinition, virtualPath) => false;

            var nextCacheDependency = this.SetNextNode(currentNode);

            // Act
            var result = currentNode.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(AggregateCacheDependency), "The result of GetCacheDependency methods should be instance of AggregateCacheDependency");
            Assert.AreSame(nextCacheDependency, result, "The returned cache dependency is not that of the second node.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether Open method returns the result of Open on the next node if the resource does not exist on the current node.")]
        public void Open_CurrentExistsFalse_ReturnsNextOpenResult()
        {
            // Arrange
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

            // Act
            var result = currentNode.Open(new PathDefinition(), "~/Test");

            // Assert
            Assert.AreSame(nextStream, result, "The returned stream is not that of the second node.");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the aggregated dependencies.
        /// </summary>
        /// <param name="aggregatedDependency">The aggregated dependency.</param>
        /// <returns></returns>
        private ArrayList GetAggregatedDependencies(AggregateCacheDependency aggregatedDependency)
        {
            var fieldInfo = typeof(AggregateCacheDependency).GetField("_dependencies", BindingFlags.Instance | BindingFlags.NonPublic);

            return (ArrayList)fieldInfo.GetValue(aggregatedDependency);
        }

        /// <summary>
        ///     Sets the next node.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <returns></returns>
        private CacheDependency SetNextNode(DummyResourceResolverNode currentNode)
        {
            var nextNode = new DummyResourceResolverNode();

            var nextCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            nextNode.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcDate) => nextCacheDependency;
            nextNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;

            currentNode.SetNext(nextNode);

            return nextCacheDependency;
        }

        #endregion
    }
}