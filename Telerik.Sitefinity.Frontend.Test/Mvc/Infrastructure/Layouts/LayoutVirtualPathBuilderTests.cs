using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that LayoutVirtualPathBuilder class is working correctly.
    /// </summary>
    [TestClass]
    public class LayoutVirtualPathBuilderTests
    {
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether BuildPathFromTitle prepends layout suffix and appends master suffix.")]
        public void BuildPathFromTitle_TemplateTitle_ConstructsVirtualPath()
        {
            //Arrange
            var templateTitle = "TestTitle";
            var expectedVirtualPath = "~/SfLayouts/TestTitle.master";

            //Act
            var layoutVirtualPathBuilder = new LayoutVirtualPathBuilder();
            var resultVirtualPath = layoutVirtualPathBuilder.BuildPathFromTitle(templateTitle);

            //Assert
            Assert.AreEqual(expectedVirtualPath, resultVirtualPath, "The virtual path is not constructed correctly.");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutName returns correct layout file name and strips additional .")]
        public void GetLayoutName_Virtualpath_ResolvesLayoutFileName()
        {
            //Arrange
            var expectedLayoutName = "TestTitle";
            var testVirtualPath = "~/TestPrefix/TestTitle....master";
            var pathDefinition = new PathDefinition();
            pathDefinition.VirtualPath = "~/TestPrefix";

            //Act
            var layoutVirtualPathBuilder = new LayoutVirtualPathBuilder();
            var resultLayoutName = layoutVirtualPathBuilder.GetLayoutName(pathDefinition, testVirtualPath);

            //Assert
            Assert.AreEqual(expectedLayoutName, resultLayoutName, "The layout name is not extracted correctly.");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutName returns null when the virtual path name doesn't end with 'master'.")]
        public void GetLayoutName_WithoutMasterSuffix_ReturnsNull()
        {
            //Arrange
            var testVirtualPath = "~/TestPrefix/TestTitle.test";
            var pathDefinition = new PathDefinition();
            pathDefinition.VirtualPath = "~/TestPrefix";

            //Act
            var layoutVirtualPathBuilder = new LayoutVirtualPathBuilder();
            var resultLayoutName = layoutVirtualPathBuilder.GetLayoutName(pathDefinition, testVirtualPath);

            //Assert
            Assert.IsNull(resultLayoutName, "The result layout name should be null if the virtual path doesn't end with 'master'.");
        }

    }
}
