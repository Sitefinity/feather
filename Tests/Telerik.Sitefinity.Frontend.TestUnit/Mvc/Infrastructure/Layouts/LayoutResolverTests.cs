using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Layouts;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.PageTemplates;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that LayoutResolver class is working correctly.
    /// </summary>
    [TestClass]
    public class LayoutResolverTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get virtual path_ hybrid mode template_ returns empty virtual path.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetVirtualPath methods returns empty string when template is not pure MVC.")]
        public void GetVirtualPath_HybridModeTemplate_ReturnsEmptyVirtualPath()
        {
            // Arrange
            var resolver = new LayoutResolver();
            var dummyPageTemplate = new DummyFrameworkSpecificPageTemplate(PageTemplateFramework.Hybrid);

            // Act
            var resolvedVirtualPath = resolver.GetVirtualPath(dummyPageTemplate);

            // Assert
            Assert.IsNull(resolvedVirtualPath, "The resolved virtual path should be null when the page template is not in MVC mode.");
        }

        /// <summary>
        /// The get virtual path_ i page template_ returns null.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetVirtualPath methods returns null if the template doesn't implement IHasTitle.")]
        public void GetVirtualPath_IPageTemplate_ReturnsNull()
        {
            // Arrange
            var resolver = new LayoutResolver();
            var dummyPageTemplate = new DummyFrameworkSpecificPageTemplate(PageTemplateFramework.Mvc);

            using (var objFactory = new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                
                Config.RegisterSection<VirtualPathSettingsConfig>();
                Config.RegisterSection<ControlsConfig>();

                VirtualPathManager.AddVirtualFileResolver<DummyVirtualFileResolver>(TestVp, "DummyVirtualFileResolver");

                // Act
                try
                {
                    string resolvedVirtualPath = resolver.GetVirtualPath(dummyPageTemplate);

                    // Assert
                    Assert.IsNull(resolvedVirtualPath, "Resolved VirtualPath should be null.");
                }
                finally
                {
                    VirtualPathManager.RemoveVirtualFileResolver(TestVp);
                }
            }
        }

        /// <summary>
        /// The get virtual path_ invalid characters_ returns clean virtual path.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether invalid characters are trimmed when constructing virtual path.")]
        public void GetVirtualPath_InvalidCharacters_ReturnsCleanVirtualPath()
        {
            // Arrange
            var dirtyTitle = "Some<>*Test:?Title";
            var cleanVirtualPath = "~/SfLayouts/Some_Test_Title.master";
            string resolvedVirtualPath;

            var resolver = new DummyLayoutResolver();

            using (var objFactory = new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                
                Config.RegisterSection<VirtualPathSettingsConfig>();
                Config.RegisterSection<ControlsConfig>();
                VirtualPathManager.AddVirtualFileResolver<DummyVirtualFileResolver>(TestVp, "DummyVirtualFileResolver");

                var dummyPageTemplate = new DummyPageTemplateWithTitle(PageTemplateFramework.Mvc, dirtyTitle);

                // Act
                try
                {
                    resolvedVirtualPath = resolver.GetVirtualPath(dummyPageTemplate);

                    // Assert
                    Assert.AreEqual(cleanVirtualPath, resolvedVirtualPath, "GetVirtualPath method doesn't return correct string.");
                }
                finally
                {
                    VirtualPathManager.RemoveVirtualFileResolver(TestVp);
                }
            }
        }

        #endregion

        #region Constants

        /// <summary>
        /// Virtual path used for test purposes only.
        /// </summary>
        private const string TestVp = "~/SfLayouts/*";

        #endregion
    }
}