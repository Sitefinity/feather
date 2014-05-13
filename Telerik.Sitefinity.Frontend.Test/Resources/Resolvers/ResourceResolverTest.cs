using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;

namespace Telerik.Sitefinity.Frontend.Test.Resources.Resolvers
{
    [TestClass]
    public class ResourceResolverTest
    {
        #region Exists
        
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Check if Exists returns false when there are no registered nodes in the resource resolver strategy.")]
        public void Exists_NoNodes_ReturnsFalse()
        {
            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());
                ObjectFactory.Resolve<IResourceResolverStrategy>().SetFirst(null);
                
                var resolver = new ResourceResolver();

                var result = resolver.Exists(new PathDefinition(), "~/Test");
                Assert.IsFalse(result, "Resolver returned true for Exists when for all nodes Exists is false.");
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if Exists returns true if there are two nodes in the strategy and the second one contains the requested resource.")]
        public void Exists_SecondNodeExists_ReturnsTrue()
        {
            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());

                var firstNode = new DummyResourceResolverNode();
                firstNode.CurrentExistsMock = (pathDefinition, virtualPath) => false;

                var secondNode = new DummyResourceResolverNode();
                secondNode.CurrentExistsMock = (pathDefinition, virtualPath) => true;

                ObjectFactory.Resolve<IResourceResolverStrategy>()
                    .SetFirst(firstNode)
                    .SetNext(secondNode);

                var resolver = new ResourceResolver();
                var result = resolver.Exists(new PathDefinition(), "~/Test");

                Assert.IsTrue(result, "Resolver returned false for Exists while there is one node for which the resource exits.");
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Check if when Exists is called with virtual path that has params, the strategy's Exists method is called without virtual path params.")]
        public void Exists_PathWithParams_StrategyCalledWithoutParams()
        {
            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, DummyResolverStrategy>(new ContainerControlledLifetimeManager());
                var strategy = (DummyResolverStrategy)ObjectFactory.Resolve<IResourceResolverStrategy>();
                
                string calledVirtualPath = null;
                strategy.ExistsMock = (definition, virtualPath) =>
                    {
                        calledVirtualPath = virtualPath;
                        return true;
                    };

                new ResourceResolver().Exists(new PathDefinition(), "~/Test#TestParams");
                Assert.AreEqual("~/Test", calledVirtualPath);
            }
        }

        #endregion

        #region Open
        
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks that if two nodes in the strategy have the requested resource the Open method will return the stream of the first one.")]
        public void Open_TwoResolvers_ReturnsFirstResolverStream()
        {
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

                        ObjectFactory.Resolve<IResourceResolverStrategy>()
                            .SetFirst(firstNode)
                            .SetNext(secondNode);

                        var resolver = new ResourceResolver();
                        var result = resolver.Open(new PathDefinition(), "~/Test");

                        Assert.AreNotSame(ms2, result, "The stream for the second node was returned while the first one was expected.");
                        Assert.AreSame(ms1, result, "The stream for the first node was not returned.");
                    }
                }
            }
        }

        #endregion

        #region GetCacheDependency

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetCacheDependency method will return the result of the resource resolver strategy's GetCacheDependnecy method.")]
        public void GetCacheDependency_ReturnsStrategysGetCacheDependncyResult()
        {
            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, DummyResolverStrategy>(new ContainerControlledLifetimeManager());
                var strategy = (DummyResolverStrategy)ObjectFactory.Resolve<IResourceResolverStrategy>();

                var strategyCacheDependency = new CacheDependency(Directory.GetCurrentDirectory());
                strategy.GetCacheDependencyMock = (definition, virtualPath, virtualPathDependencies, utcStart) => strategyCacheDependency;

                var resolver = new ResourceResolver();
                var result = resolver.GetCacheDependency(new PathDefinition(), "~/Test", null, DateTime.UtcNow);

                Assert.AreSame(strategyCacheDependency, result, "GetCacheDependency did not return the dependency that was provided by the resource resolver strategy.");
            }
        }

        #endregion
    }
}
