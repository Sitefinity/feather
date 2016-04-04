using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Layouts;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that LayoutVirtualPathBuilder class is working correctly.
    /// </summary>
    [TestClass]
    public class LayoutVirtualPathBuilderTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The build path from title_ template title_ constructs virtual path.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether BuildPathFromTitle prepends layout suffix and appends master suffix.")]
        public void BuildPathFromTitle_TemplateTitle_ConstructsVirtualPath()
        {
            // Arrange
            var templateTitle = "TestTitle";
            var expectedVirtualPath = "~/SfLayouts/TestTitle.master";

            // Act
            var layoutVirtualPathBuilder = new DummyLayoutVirtualPathBuilder();
            var resultVirtualPath = layoutVirtualPathBuilder.BuildPathFromName(templateTitle);

            // Assert
            Assert.AreEqual(expectedVirtualPath, resultVirtualPath, "The virtual path is not constructed correctly.");
        }

        /// <summary>
        /// The get layout name_ virtualpath_ resolves layout file name.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutName returns correct layout file name and strips additional .")]
        public void GetLayoutName_VirtualPath_ResolvesLayoutFileName()
        {
            // Arrange
            var expectedLayoutName = "TestTitle";
            var testVirtualPath = "~/TestPrefix/TestTitle....master";
            var pathDefinition = new PathDefinition { VirtualPath = "~/TestPrefix" };

            // Act
            var layoutVirtualPathBuilder = new LayoutVirtualPathBuilder();
            var resultLayoutName = layoutVirtualPathBuilder.GetLayoutName(pathDefinition, testVirtualPath);

            // Assert
            Assert.AreEqual(expectedLayoutName, resultLayoutName, "The layout name is not extracted correctly.");
        }

        /// <summary>
        /// The get layout name_ without master suffix_ returns null.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutName returns null when the virtual path name doesn't end with 'master'.")]
        public void GetLayoutName_WithoutMasterSuffix_ReturnsNull()
        {
            // Arrange
            var testVirtualPath = "~/TestPrefix/TestTitle.test";
            var pathDefinition = new PathDefinition();
            pathDefinition.VirtualPath = "~/TestPrefix";

            // Act
            var layoutVirtualPathBuilder = new LayoutVirtualPathBuilder();
            var resultLayoutName = layoutVirtualPathBuilder.GetLayoutName(pathDefinition, testVirtualPath);

            // Assert
            Assert.IsNull(resultLayoutName, "The result layout name should be null if the virtual path doesn't end with 'master'.");
        }

        #endregion
    }
}