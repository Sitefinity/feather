using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources
{
    [TestClass]
    public class RazorTemplateProcessorTests
    {
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether the Run will use cached templates on multiple calls.")]
        public void Run_MultipleCalls_ReturnsCachedParsedTemplate()
        {
            // Arrange
            var templatePath = "some-path/my-template.sf-cshtml";
            var virtualPathProvider = new DummyVirtualPathProvider();
            virtualPathProvider.Content[templatePath] = "my template";

            var templateService = new DummyTemplateService(s => s + " ran");
            var processor = new RazorTemplateProcessor(templateService, virtualPathProvider);

            // Act
            var resultFirst = processor.Run(templatePath, null);
            virtualPathProvider.Content[templatePath] = "my template modified";
            var resultSecond = processor.Run(templatePath, null);

            // Assert
            Assert.AreEqual("my template ran", resultFirst, "Template was not parsed as expected.");
            Assert.AreEqual("my template ran", resultSecond, "The template was compiled a second time without any need.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether the Run will use an updated version of a template when modified.")]
        public void Run_MultipleCallsModifiedTemplate_ReturnsModifiedTemplate()
        {
            // Arrange
            var templatePath = "some-path/my-template.sf-cshtml";
            var virtualPathProvider = new DummyVirtualPathProvider();
            virtualPathProvider.Content[templatePath] = "my template";

            var templateService = new DummyTemplateService(s => s + " ran");
            var processor = new RazorTemplateProcessor(templateService, virtualPathProvider);

            // Act
            var resultFirst = processor.Run(templatePath, null);
            virtualPathProvider.Content[templatePath] = "my template modified";
            virtualPathProvider.Dependencies[templatePath].Change();

            var resultSecond = processor.Run(templatePath, null);

            // Assert
            Assert.AreEqual("my template ran", resultFirst);
            Assert.AreEqual("my template modified ran", resultSecond);
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RazorTemplateProcessor")]
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether the Run will return exception as string if thrown by the template service.")]
        public void Run_ParseException_ReturnsExceptionString()
        {
            // Arrange
            var ex = new Exception("Template service threw an exception that RazorTemplateProcessor failed to catch.");

            var templatePath = "some-path/my-template.sf-cshtml";
            var virtualPathProvider = new DummyVirtualPathProvider();
            virtualPathProvider.Content[templatePath] = "my template";

            var templateService = new DummyTemplateService(s =>
            {
                throw ex;
            });
            var processor = new RazorTemplateProcessor(templateService, virtualPathProvider);

            // Act
            var result = processor.Run(templatePath, null);

            // Assert
            Assert.AreEqual(ex.ToString(), result, "Run did not return the expected result.");
        }
    }
}
