using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        #region GetTemplateFramework

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the GetTemplateFramework method returns Hybrid if the template doesn't implement IFrameworkSpecificPageTemplate")]
        public void GetTemplateFramework_NoSpecificFremework_ReturnsHybrid()
        {
            //Arrange
            DummyPageTemplate pTemplate = new DummyPageTemplate();

            //Act
            var framework = pTemplate.GetTemplateFramework();

            //Assert
            Assert.AreEqual(PageTemplateFramework.Hybrid, framework, "The framework of the template should be Hybrid.");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the GetTemplateFramework method returns the correct framework if the template is IFrameworkSpecificPageTemplate")]
        public void GetTemplateFramework_SpecificFremework_ReturnsCorrectFramework()
        {
            //Arrange
            DummyFrameworkSpecificPageTemplate pTemplate = new DummyFrameworkSpecificPageTemplate(PageTemplateFramework.Mvc);
            
            //Act
            var framework = pTemplate.GetTemplateFramework();

            //Assert
            Assert.AreEqual(PageTemplateFramework.Mvc, framework, "The framework of the template should be Mvc.");
        }

        #endregion
    }
}
