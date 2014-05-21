using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.LocalizationResources;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Project.Configuration;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;


namespace Telerik.Sitefinity.Frontend.Test.Mvc.Helpers
{
    /// <summary>
    /// Tests methods of the LocalizationHelpers class.
    /// </summary>
    [TestClass]
    public class LocalizationHelpersTests
    {
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether  the LocalizationHelpers returns a proper resource value for localized controller.")]
        public void LocalizationHelpersTests_GetLabelForDummyLocalizedController_EnsuresTheResourceStringIsFound()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();
            var controller = typeof(DummyLocalizedController);

            ViewContext context = new ViewContext();
            context.Controller = new DummyLocalizedController();
            var urlHelper = new HtmlHelper(context, new DummyViewDataContainer());


            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(),
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<ResourcesConfig>();
                Config.RegisterSection<ProjectConfig>();

                //Act
                initializer.RegisterControllerPublic(controller);
                var resourceString = urlHelper.SfRes("DummyResource");

                //Assert
                var resourceRegistered = ObjectFactory.Container.IsRegistered(typeof(DummyLocalizationControllerResources), Res.GetResourceClassId(typeof(DummyLocalizationControllerResources)));
                Assert.IsTrue(resourceRegistered, "String resources were not registered for the controller.");
                Assert.IsFalse(resourceString.IsNullOrEmpty(), "The resource with the given key was not found");
                Assert.AreEqual<string>("Dummy Resource", resourceString, "The returned resource is not as expected");
            }
        }

    }
}
