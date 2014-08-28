using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.PageTemplates;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that PageTemplateExtensions class is working correctly.
    /// </summary>
    [TestClass]
    public class PageTemplateExtensionsTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get template framework_ no specific fremework_ returns hybrid.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the GetTemplateFramework method returns Hybrid if the template doesn't implement IFrameworkSpecificPageTemplate")]
        public void GetTemplateFramework_NoSpecificFramework_ReturnsHybrid()
        {
            // Arrange
            var template = new DummyPageTemplate();

            // Act
            var framework = template.GetTemplateFramework();

            // Assert
            Assert.AreEqual(PageTemplateFramework.Hybrid, framework, "The framework of the template should be Hybrid.");
        }

        /// <summary>
        /// The get template framework_ specific fremework_ returns correct framework.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the GetTemplateFramework method returns the correct framework if the template is IFrameworkSpecificPageTemplate")]
        public void GetTemplateFramework_SpecificFramework_ReturnsCorrectFramework()
        {
            // Arrange
            var template = new DummyFrameworkSpecificPageTemplate(PageTemplateFramework.Mvc);

            // Act
            var framework = template.GetTemplateFramework();

            // Assert
            Assert.AreEqual(PageTemplateFramework.Mvc, framework, "The framework of the template should be Mvc.");
        }

        #endregion
    }
}