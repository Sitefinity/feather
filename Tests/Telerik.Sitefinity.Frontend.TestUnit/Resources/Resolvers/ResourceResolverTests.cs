using System;
using System.IO;
using System.Web.Caching;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources.Resolvers
{
    /// <summary>
    /// Ensures that the ResourceResolver class is working correctly.
    /// </summary>
    [TestClass]
    public class ResourceResolverTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The exists_ no nodes_ returns false.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Check if Exists returns false when there are no registered nodes in the resource resolver strategy.")]
        public void Exists_NoNodes_ReturnsFalse()
        {
            using (new ObjectFactoryContainerRegion())
            {
                // Arrange
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());
                ObjectFactory.Resolve<IResourceResolverStrategy>().SetFirst(null);

                var resolver = new ResourceResolver();

                // Act
                var result = resolver.Exists(new PathDefinition(), "~/Test");

                // Assert
                Assert.IsFalse(result, "Resolver returned true for Exists when for all nodes Exists is false.");
            }
        }

        /// <summary>
        /// The exists_ path with params_ strategy called without params.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Check if when Exists is called with virtual path that has params, the strategy's Exists method is called without virtual path params.")]
        public void Exists_PathWithParams_StrategyCalledWithoutParams()
        {
            using (new ObjectFactoryContainerRegion())
            {
                // Arrange
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, DummyResolverStrategy>(new ContainerControlledLifetimeManager());
                var strategy = (DummyResolverStrategy)ObjectFactory.Resolve<IResourceResolverStrategy>();

                string calledVirtualPath = null;

                strategy.ExistsMock = (definition, virtualPath) =>
                    {
                        calledVirtualPath = virtualPath;
                        return true;
                    };

                // Act
                new ResourceResolver().Exists(new PathDefinition(), "~/Test#TestParams");

                // Assert
                Assert.AreEqual("~/Test", calledVirtualPath, "Called resource doesn't have the expected virtual path.");
            }
        }

        /// <summary>
        /// The exists_ second node exists_ returns true.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if Exists returns true if there are two nodes in the strategy and the second one contains the requested resource.")]
        public void Exists_SecondNodeExists_ReturnsTrue()
        {
            using (new ObjectFactoryContainerRegion())
            {
                // Arrange
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());

                var firstNode = new DummyResourceResolverNode();
                firstNode.CurrentExistsMock = (pathDefinition, virtualPath) => false;

                var secondNode = new DummyResourceResolverNode();
                secondNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;

                ObjectFactory.Resolve<IResourceResolverStrategy>().SetFirst(firstNode).SetNext(secondNode);

                // Act
                var resolver = new ResourceResolver();
                bool result = resolver.Exists(new PathDefinition(), "~/Test");

                // Assert
                Assert.IsTrue(result, "Resolver returned false for Exists while there is one node for which the resource exits.");
            }
        }

        /// <summary>
        /// The get cache dependency_ returns strategys get cache dependncy result.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency method will return the result of the resource resolver strategy's GetCacheDependnecy method.")]
        public void GetCacheDependency_ReturnsStrategiesGetCacheDependencyResult()
        {
            using (new ObjectFactoryContainerRegion())
            {
                // Arrange
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, DummyResolverStrategy>(new ContainerControlledLifetimeManager());
                var strategy = (DummyResolverStrategy)ObjectFactory.Resolve<IResourceResolverStrategy>();

                var strategyCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
                strategy.GetCacheDependencyMock = (definition, virtualPath, virtualPathDependencies, utcStart) => strategyCacheDependency;

                // Act
                var resolver = new ResourceResolver();
                var result = resolver.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);

                // Assert
                Assert.AreSame(strategyCacheDependency, result, "GetCacheDependency did not return the dependency that was provided by the resource resolver strategy.");
            }
        }

        /// <summary>
        /// The open_ two resolvers_ returns first resolver stream.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks that if two nodes in the strategy have the requested resource the Open method will return the stream of the first one.")]
        public void Open_TwoResolvers_ReturnsFirstResolverStream()
        {
            // Arrange
            using (new ObjectFactoryContainerRegion())
            {
                using (var ms1 = new MemoryStream())
                {
                    using (var ms2 = new MemoryStream())
                    {
                        ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());

                        var firstNode = new DummyResourceResolverNode();
                        firstNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;
                        firstNode.CurrentOpenMock = (pathDefinition, virtualPath) => ms1;

                        var secondNode = new DummyResourceResolverNode();
                        secondNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;
                        secondNode.CurrentOpenMock = (pathDefinition, virtualPath) => ms2;

                        ObjectFactory.Resolve<IResourceResolverStrategy>().SetFirst(firstNode).SetNext(secondNode);

                        // Act
                        var resolver = new ResourceResolver();
                        var result = resolver.Open(new PathDefinition(), "~/Test");

                        // Assert
                        Assert.AreNotSame(ms2, result, "The stream for the second node was returned while the first one was expected.");
                        Assert.AreSame(ms1, result, "The stream for the first node was not returned.");
                    }
                }
            }
        }

        #endregion
    }
}