using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Telerik.Sitefinity.Frontend.Designers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Controls;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;

namespace Telerik.Sitefinity.Frontend.Test.Designers
{
    /// <summary>
    /// Ensures that DesignerResolver class works correctly.
    /// </summary>
    [TestClass]
    public class DesignerResolverTests
    {
        #region GetUrl

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetUrl throws exception if it is invoked without providing widget type.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUrl_WithoutWidgetType_ThrowsException()
        {
            var resolver = new DesignerResolver();
            var url = resolver.GetUrl(null);
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns the default MVC designer URL.")]
        public void GetUrl_Controller_ReturnsDefaultMvcDesignerUrl()
        {
            //Arrange
            var resolver = new DesignerResolver();

            //Act
            var url = resolver.GetUrl(typeof(DummyController));

            //Assert
            Assert.AreEqual("~/Telerik.Sitefinity.Frontend/Designer/Designer/Dummy", url, "The default designer URL is not retrieved properly.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns the custom designer URL which is set in the MVC designer attribute.")]
        public void GetUrl_MvcCustomDesignerController_ReturnsCustomDesignerUrl()
        {
            //Arrange
            var resolver = new DesignerResolver();

            //Act
            var url = resolver.GetUrl(typeof(DummyCustomDesignerController));

            //Assert
            Assert.AreEqual(DummyCustomDesignerController.CustomDesignerUrl, url, "The designer URL specified in the controller's attribute is not retrieved properly.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns null for a System.Web.UI.Control.")]
        public void GetUrl_Control_ReturnsNull()
        {
            //Arrange
            var resolver = new DesignerResolver();

            //Act
            var url = resolver.GetUrl(typeof(DummyControl));

            //Assert
            Assert.IsNull(url, "The default designer URL for WebForms widget should be null.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns the default MVC designer URL when MVC designer URL is set on a Control.")]
        public void GetUrl_ControlWithMvcDesigner_ReturnsMvcDesignerUrl()
        {
            //Arrange
            var resolver = new DesignerResolver();

            //Act
            var url = resolver.GetUrl(typeof(DummyMvcDesignerControl));

            //Assert
            Assert.AreEqual(DummyMvcDesignerControl.CustomDesignerUrl, url,
                "The specified designer URL should be returned for WebForms widget when DesignerUrl attribute is set.");
        }

        #endregion
    }
}
