using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that PageTemplateExtensions class is working correctly.
    /// </summary>
    [TestClass]
    public class PageTemplateExtensionsTests
    {
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the GetTemplateFramework method returns Hybrid if the template doesn't implement IFrameworkSpecificPageTemplate")]
        public void GetTemplateFramework_NoSpecificFremework_ReturnsHybrid()
        {
            DummyPageTemplate pTemplate = new DummyPageTemplate();
            var framework = pTemplate.GetTemplateFramework();

            Assert.AreEqual(PageTemplateFramework.Hybrid, framework);
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the GetTemplateFramework method returns the correct framework if the template is IFrameworkSpecificPageTemplate")]
        public void GetTemplateFramework_SpecificFremework_ReturnsCorrectFramework()
        {
            DummyFrameworkSpecificPageTemplate pTemplate = new DummyFrameworkSpecificPageTemplate(PageTemplateFramework.Mvc);
            var framework = pTemplate.GetTemplateFramework();

            Assert.AreEqual(PageTemplateFramework.Mvc, framework);
        }
    }
}
