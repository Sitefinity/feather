using System;
using System.Web.Mvc;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Helpers;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Project.Configuration;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Helpers
{
    /// <summary>
    /// Tests methods of the LocalizationHelpers class.
    /// </summary>
    [TestClass]
    public class LocalizationHelpersTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get label_ dummy localized controller_ ensures the resource string is found.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the LocalizationHelpers returns a proper resource value for localized controller.")]
        public void GetLabel_DummyLocalizedController_EnsuresTheResourceStringIsFound()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();
            Type controller = typeof(DummyLocalizedController);

            var context = new ViewContext();
            context.Controller = new DummyLocalizedController();
            var urlHelper = new HtmlHelper(context, new DummyViewDataContainer());

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                ObjectFactory.Container.RegisterType<ICacheManager, NoCacheManager>(CacheManagerInstance.LocalizationResources.ToString());
                Config.RegisterSection<ResourcesConfig>();
                Config.RegisterSection<ProjectConfig>();
                Config.RegisterSection<SystemConfig>();

                // Act
                initializer.RegisterControllerPublic(controller);

                var resourceString = urlHelper.Resource("DummyResource");

                // Assert
                var resourceRegistered = ObjectFactory.Container.IsRegistered(typeof(DummyLocalizationControllerResources), Res.GetResourceClassId(typeof(DummyLocalizationControllerResources)));
                Assert.IsTrue(resourceRegistered, "String resources were not registered for the controller.");
                Assert.IsFalse(resourceString.IsNullOrEmpty(), "The resource with the given key was not found");
                Assert.AreEqual("Dummy Resource", resourceString, "The returned resource is not as expected");
            }
        }

        #endregion
    }
}