﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Web.Caching;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Test.Resources.Resolvers
{
    /// <summary>
    /// Tests methods of DatabaseResourceResolver class.
    /// </summary>
    [TestClass]
    public class DatabaseResourceResolverTests
    {
        #region Exists

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether Exists method returns True for existing resource.")]
        public void Exists_ExistingResource_ReturnsTrue()
        {
            //Arrange
            const string virtualPath = "Test/MyTemplate.cshtml";
            var resolver = new DummyDatabaseResourceResolver();
            resolver.ControlPresentationResult[virtualPath] = new ControlPresentation() { Data = "<div>Content</div>" };

            //Act
            var result = resolver.Exists(new PathDefinition(), virtualPath);

            //Assert
            Assert.IsTrue(result, "DatabaseResourceResolver.Exists returned False for an existing resource.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether Exists method returns False for non-existing resource.")]
        public void Exists_NonExistingResource_ReturnsFalse()
        {
            //Arrange
            var resolver = new DummyDatabaseResourceResolver();

            //Act
            var result = resolver.Exists(new PathDefinition(), "Test/MyTemplate.cshtml");

            //Assert
            Assert.IsFalse(result, "DatabaseResourceResolver.Exists returned True for a non-existing resource.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether Exists method returns False for resource with empty content.")]
        public void Exists_ResourceWithNullContent_ReturnsFalse()
        {
            //Arrange
            const string virtualPath = "Test/MyTemplate.cshtml";
            var resolver = new DummyDatabaseResourceResolver();
            resolver.ControlPresentationResult[virtualPath] = new ControlPresentation() { Data = null };

            //Act
            var result = resolver.Exists(new PathDefinition(), virtualPath);

            //Assert
            Assert.IsFalse(result, "DatabaseResourceResolver.Exists returned True for resource with null content.");
        }

        #endregion

        #region GetCacheDependency
        
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether GetCacheDependency method returns ControlPresentationCacheDependency associated with Id of an existing resource.")]
        public void GetCacheDependency_ExistingResource_ReturnsDependencyWithId()
        {
            //Arrange
            const string virtualPath = "Test/MyTemplate.cshtml";
            Guid presentationId = new Guid("4B9F6715-014E-4B1E-887F-428AF942EAC8");

            var resolver = new DummyDatabaseResourceResolver();
            resolver.ControlPresentationResult[virtualPath] = new ControlPresentation(null, presentationId);

            //Act
            var presentationDependency = this.GetCacheDependencyResult(resolver, virtualPath);

            //Assert
            var itemId = presentationDependency.GetType().GetField("itemId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(presentationDependency) as string;
            Assert.AreEqual(presentationId.ToString().ToLower(), itemId.ToLower(), 
                "GetCacheDependency did not return dependency on the expected object.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether GetCacheDependency method returns ControlPresentationCacheDependency without key for non existing resource.")]
        public void GetCacheDependency_NonExistingResource_ReturnsDependencyWithoutId()
        {
            //Arrange
            const string virtualPath = "Test/MyTemplate.cshtml";
            var resolver = new DummyDatabaseResourceResolver();

            //Act
            var presentationDependency = this.GetCacheDependencyResult(resolver, virtualPath);

            //Assert
            var itemId = presentationDependency.GetType().GetField("itemId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(presentationDependency) as string;
            Assert.IsNull(itemId, "The returned CacheDependency has an unexpected key.");
        }

        #endregion

        #region Open

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether Open method will return a stream with the resource Data for existing resource.")]
        public void Open_ExistingResource_StreamsResourceData()
        {
            //Arrange
            const string virtualPath = "Test/MyTemplate.cshtml";
            const string content = "<div>Content</div>";

            //Act
            var resolver = new DummyDatabaseResourceResolver();
            resolver.ControlPresentationResult[virtualPath] = new ControlPresentation() { Data = content };
            var result = resolver.Open(new PathDefinition(), virtualPath);

            //Assert
            Assert.IsNotNull(result, "Open method returned null.");

            using (var reader = new StreamReader(result))
            {
                var returnedData = reader.ReadToEnd();
                Assert.AreEqual(content, returnedData, "The stream did not contain the expected data.");
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether Open method will throw ArgumentException for non-existing resource.")]
        [ExpectedException(typeof(ArgumentException))]
        public void Open_NonExistingResource_ThrowsArgumentException()
        {
            //Arrange
            const string virtualPath = "Test/MyTemplate.cshtml";

            //Act
            new DummyDatabaseResourceResolver().Open(new PathDefinition(), virtualPath);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Gets the cache dependency result.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        private ControlPresentationCacheDependency GetCacheDependencyResult(DummyDatabaseResourceResolver resolver, string virtualPath)
        {
            CacheDependency result = null;

            using (var objFactory = new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(),
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<SystemConfig>();

                result = resolver.GetCacheDependency(new PathDefinition(), virtualPath, null, DateTime.UtcNow);
            }

            Assert.IsInstanceOfType(result, typeof(ControlPresentationCacheDependency), "Returned cache dependency was not of the expected type.");

            var presentationDependency = (ControlPresentationCacheDependency)result;

            return presentationDependency;
        }

        #endregion 
    }
}
