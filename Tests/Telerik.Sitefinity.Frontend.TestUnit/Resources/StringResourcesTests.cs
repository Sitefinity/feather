using System;
using System.Linq;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.Mvc.StringResources;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Project.Configuration;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources
{
    /// <summary>
    /// This class contains test for the string resources.
    /// </summary>
    [TestClass]
    public class StringResourcesTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Owner("EGaneva")]
        [Description("The test ensures that designer resources are correct.")]
        public void DesignerResources_IterateTheResources_AssureResourcesAreCorrect()
        {
            // Act & Assert: Iterate over each resource property and verify its correctness 
            this.TestResourceType<DesignerResources>();
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("The test ensures that client component resources are correct.")]
        public void ClientComponentsResources_IterateTheResources_AssureResourcesAreCorrect()
        {
            // Act & Assert: Iterate over each resource property and verify its correctness 
            this.TestResourceType<ClientComponentsResources>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tests a given type of resource.
        /// </summary>
        private void TestResourceType<TRes>() where TRes : Resource, new()
        {
            // Arrange: Use the  getResourceClassDelegate to register and obtain a resource class instance, get the resource class type, register a dummy Config provider
            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                ObjectFactory.Container.RegisterType<ICacheManager, NoCacheManager>(CacheManagerInstance.LocalizationResources.ToString());

                Config.RegisterSection<ResourcesConfig>();
                Config.RegisterSection<ProjectConfig>();
                Config.RegisterSection<SystemConfig>();

                Type resourceClassType = typeof(TRes);
                Res.RegisterResource(resourceClassType);

                var resourceClass = Res.Get<TRes>();
                Assert.IsNotNull(resourceClass, "The resource class cannot be instantiated.");

                // Act & Assert: Iterate over each resource and verify if its resource attribute is correct and if the resource value is correct 
                var properties = resourceClassType.GetProperties().Where(p => p.GetCustomAttributes(typeof(ResourceEntryAttribute), false).Count() == 1);
                
                foreach (var prop in properties)
                {
                    var attribute = prop.GetCustomAttributes(typeof(ResourceEntryAttribute), false).FirstOrDefault() as ResourceEntryAttribute;

                    Assert.IsNotNull(attribute, "The resource property does not have the required resource attribute.");
                    
                    var resource = prop.GetValue(resourceClass, null) as string;

                    Assert.IsFalse(string.IsNullOrEmpty(resource), string.Format(System.Globalization.CultureInfo.InvariantCulture, "The resource string for the {0} property cannot be found,", prop.Name));
                    Assert.AreEqual(prop.Name, attribute.Key, "The resource key does not match the property name,");
                    Assert.AreEqual(resource, attribute.Value, string.Format(System.Globalization.CultureInfo.InvariantCulture, "The resource string for the {0} property cannot be found,", prop.Name));
                    Assert.IsFalse(string.IsNullOrEmpty(attribute.Description), "The description of the resource cannot be empty string.");
                }
            }
        }

        #endregion
    }
}