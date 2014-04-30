using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Data.Metadata;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.Test.DummyClasses;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web.Configuration;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that LayoutResolver class is working correctly.
    /// </summary>
    [TestClass]
    public class LayoutResolverTests
    {
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether invalid characters are trimmed when constructing virtual path.")]
        public void GetVirtualPath_InvalidCharacters_ReturnsCleanVirtualPath()
        {
            //Arrange
            string dirtyTitle = "Some<>*Test:?Title";
            string cleanVirtualPath = "~/SfLayouts/Some_Test_Title.master";
            string resolvedVirtualPath;
            LayoutResolver resolver = new LayoutResolver();

            using (var objFactory = new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(),
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<VirtualPathSettingsConfig>();
                Config.RegisterSection<ControlsConfig>();
                VirtualPathManager.AddVirtualFileResolver<DummyVirtualFileResolver>(LayoutResolverTests.testVp, "DummyVirtualFileResolver");

                var dummyPageTemplate = new DummyPageTemplateWithTitle(PageTemplateFramework.Mvc, dirtyTitle);

                //Act
                try
                {
                    resolvedVirtualPath = resolver.GetVirtualPath(dummyPageTemplate);

                    //Assert
                    Assert.AreEqual(cleanVirtualPath, resolvedVirtualPath, "GetVirtualPath method doesn't return correct string.");
                }
                finally
                {
                    VirtualPathManager.RemoveVirtualFileResolver(LayoutResolverTests.testVp);
                }
            }
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetVirtualPath methods returns null if the template doesn't implement IHasTitle.")]
        public void GetVirtualPath_IPageTemplate_ReturnsNull()
        {
            //Arrange
            LayoutResolver resolver = new LayoutResolver();
            DummyFrameworkSpecificPageTemplate dummyPageTemplate = new DummyFrameworkSpecificPageTemplate(PageTemplateFramework.Mvc);                        

            using (var objFactory = new ObjectFactoryContainerRegion())
            {

                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(),
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<VirtualPathSettingsConfig>();
                Config.RegisterSection<ControlsConfig>();


                VirtualPathManager.AddVirtualFileResolver<DummyVirtualFileResolver>(LayoutResolverTests.testVp, "DummyVirtualFileResolver");

                //Act
                try
                {
                    var resolvedVirtualPath = resolver.GetVirtualPath(dummyPageTemplate);

                    //Assert
                    Assert.IsNull(resolvedVirtualPath, "Resolved VirtualPath should be null.");
                }
                finally
                {
                    VirtualPathManager.RemoveVirtualFileResolver(LayoutResolverTests.testVp);
                }
            }
        }


        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetVirtualPath methods returns empty string when template is not pure MVC.")]
        public void GetVirtualPath_HybridModeTemplate_ReturnsEmptyVirtualPath()
        {
            //Arrange
            LayoutResolver resolver = new LayoutResolver();
            DummyFrameworkSpecificPageTemplate dummyPageTemplate = new DummyFrameworkSpecificPageTemplate(PageTemplateFramework.Hybrid);

            //Act
            var resolvedVirtualPath = resolver.GetVirtualPath(dummyPageTemplate);

            //Assert
            Assert.IsNull(resolvedVirtualPath, "The resolved virtual path should be null when the page template is not in MVC mode.");
        }

        /// <summary>
        /// Virtual path used for test purposes only.
        /// </summary>
        private const string testVp = "~/SfLayouts/*";
    }
}
