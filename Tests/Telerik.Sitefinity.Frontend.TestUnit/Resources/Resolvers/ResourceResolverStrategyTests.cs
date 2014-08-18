using System;
using System.IO;
using System.Web.Caching;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources.Resolvers
{
    /// <summary>
    /// Ensures that the ResourceResolverStrategy class is working correctly.
    /// </summary>
    [TestClass]
    public class ResourceResolverStrategyTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency calls GetCacheDependency method of its first node and returns its result.")]
        public void GetCacheDependency_ReturnsFirstNodesCacheDependency()
        {
            // Arrange
            var strategy = new ResourceResolverStrategy();
            var nodeCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
            var node = new DummyResourceResolverNode();

            strategy.SetFirst(node);
            node.GetCurrentCacheDependencyMock = (pathDefinition, virtualPath, virtualPathDependencies, utcStart) => nodeCacheDependency;

            // Act
            var resultCacheDependency = strategy.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);

            // Assert
            Assert.AreSame(nodeCacheDependency, resultCacheDependency, "GetCacheDependency did not return the instance that is provided by the node in the strategy.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether Open throws InvalidOperationException when no nodes are present.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Open_NoNodes_ThrowsInvalidOperationException()
        {
            // Arrange
            var strategy = new ResourceResolverStrategy();
            strategy.SetFirst(null);

            // Act
            strategy.Open(new PathDefinition(), "~/Test");
        }

        #endregion
    }
}