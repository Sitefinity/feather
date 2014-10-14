using System;
using System.Web;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Designers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Controls;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.TestUnit.Designers
{
    /// <summary>
    /// Ensures that DesignerResolver class works correctly.
    /// </summary>
    [TestClass]
    public class DesignerResolverTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get url_ control with mvc designer_ returns mvc designer url.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns the default MVC designer URL when MVC designer URL is set on a Control.")]
        public void GetUrl_ControlWithMvcDesigner_ReturnsMvcDesignerUrl()
        {
            // Arrange
            var resolver = new DesignerResolver();

            // Act
            var url = resolver.GetUrl(typeof(DummyMvcDesignerControl));

            // Assert
            Assert.AreEqual(DummyMvcDesignerControl.CustomDesignerUrl, url, "The specified designer URL should be returned for WebForms widget when DesignerUrl attribute is set.");
        }

        /// <summary>
        /// The get url_ control_ returns null.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns null for a System.Web.UI.Control.")]
        public void GetUrl_Control_ReturnsNull()
        {
            // Arrange
            var resolver = new DesignerResolver();

            // Act
            var url = resolver.GetUrl(typeof(DummyControl));

            // Assert
            Assert.IsNull(url, "The default designer URL for WebForms widget should be null.");
        }

        /// <summary>
        /// The get url_ controller with package_ returns mvc designer url with package query.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns the MVC designer URL with package URL parameter when the current request has a package.")]
        public void GetUrl_ControllerWithPackage_ReturnsMvcDesignerUrlWithPackageQuery()
        {
            // Arrange
            var resolver = new DesignerResolver();
            var context =
                new HttpContextWrapper(
                    new HttpContext(
                        new HttpRequest(null, "http://tempuri.org/test?package=MyPackage", "package=MyPackage"), 
                        new HttpResponse(null)));

            // Act
            string url = null;
            SystemManager.RunWithHttpContext(context, () => { url = resolver.GetUrl(typeof(DummyController)); });

            // Assert
            Assert.AreEqual("~/Telerik.Sitefinity.Frontend/Designer/Master/Dummy?package=MyPackage", url, "The default designer URL is not retrieved properly.");
        }

        /// <summary>
        /// The get url_ controller_ returns default mvc designer url.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns the default MVC designer URL.")]
        public void GetUrl_Controller_ReturnsDefaultMvcDesignerUrl()
        {
            // Arrange
            var resolver = new DesignerResolver();

            // Act
            var url = resolver.GetUrl(typeof(DummyController));

            // Assert
            Assert.AreEqual("~/Telerik.Sitefinity.Frontend/Designer/Master/Dummy", url, "The default designer URL is not retrieved properly.");
        }

        /// <summary>
        /// The get url_ mvc custom designer controller_ returns custom designer url.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetUrl returns the custom designer URL which is set in the MVC designer attribute.")]
        public void GetUrl_MvcCustomDesignerController_ReturnsCustomDesignerUrl()
        {
            // Arrange
            var resolver = new DesignerResolver();

            // Act
            var url = resolver.GetUrl(typeof(DummyCustomDesignerController));

            // Assert
            Assert.AreEqual(DummyCustomDesignerController.CustomDesignerUrl, url, "The designer URL specified in the controller's attribute is not retrieved properly.");
        }

        /// <summary>
        /// The GetUrl for MVC controller with empty DesignerUrlAttribute returns empty designer URL.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetUrl returns the old designer URL when MVC designer attribute is set with empty URL.")]
        public void GetUrl_MvcEmptyDesignerUrl_ReturnsOldDesignerUrl()
        {
            // Arrange
            var resolver = new DesignerResolver();

            // Act
            var url = resolver.GetUrl(typeof(DummyOldDesignerController));

            // Assert
            Assert.IsTrue(url.IsNullOrEmpty(), "The old designer URL is not retrieved properly.");
        }

        /// <summary>
        /// The get url_ without widget type_ throws exception.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetUrl throws exception if it is invoked without providing widget type.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUrl_WithoutWidgetType_ThrowsException()
        {
            var resolver = new DesignerResolver();
            var url = resolver.GetUrl(null);
        }

        #endregion
    }
}