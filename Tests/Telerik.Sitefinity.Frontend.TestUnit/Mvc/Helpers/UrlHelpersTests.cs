using System;
using System.Web.Mvc;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;

namespace Telerik.Sitefinity.Frontend.Mvc.Test.Helpers
{
    /// <summary>
    /// Tests methods of the UrlHelpers class.
    /// </summary>
    [TestClass]
    public class UrlHelpersTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The widget content_ absolute path_ unchanged.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether absolute URLs are not changed by WidgetContent.")]
        public void WidgetContent_AbsolutePath_Unchanged()
        {
            // Arrange
            var dummyHttpContext = new DummyHttpContext();
            var urlHelper = new UrlHelper(dummyHttpContext.Request.RequestContext);

            // Act
            var absUrl = "http://www.sitefinity.com/";
            var result = urlHelper.WidgetContent(absUrl);

            // Assert
            Assert.AreEqual(absUrl, result);
        }

        /// <summary>
        /// The widget content_ app relative path_ resolves as normal content.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether application relative paths are resolved as usual.")]
        public void WidgetContent_AppRelativePath_ResolvesAsNormalContent()
        {
            // Arrange
            var dummyHttpContext = new DummyHttpContext();
            var urlHelper = new UrlHelper(dummyHttpContext.Request.RequestContext);

            // Act
            var expected = urlHelper.Content("~/Test");
            var result = urlHelper.WidgetContent("~/Test");

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// The widget content_ relative path and route data_ appends widget virtual path.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether WidgetContent will return a URL starting with the widget's virtual path given a relative URL and correct RouteData.")]
        public void WidgetContent_RelativePathAndRouteData_AppendsWidgetVirtualPath()
        {
            // Arrange
            var dummyHttpContext = new DummyHttpContext();
            dummyHttpContext.Request.RequestContext.RouteData.Values.Add("controller", "Dummy");
            var urlHelper = new UrlHelper(dummyHttpContext.Request.RequestContext);

            string result;
            using (var factoryReg = new ControllerFactoryRegion<DummyControllerFactory>())
            {
                factoryReg.Factory.ControllerRegistry["Dummy"] = typeof(DummyController);

                // Act
                result = urlHelper.WidgetContent("Test/MyScript.js");
            }

            // Assert
            Assert.AreEqual("/Frontend-Assembly/Telerik.Sitefinity.Frontend.TestUtilities/Test/MyScript.js", result);
        }

        /// <summary>
        /// The widget content_ relative path and widget route data_ appends widget virtual path.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether WidgetContent will return a URL containing widget resource fallback mechanisms given relative URL and widgetName in the RouteData.")]
        public void WidgetContent_RelativePathAndWidgetRouteData_AppendsWidgetVirtualPath()
        {
            // Arrange
            var dummyHttpContext = new DummyHttpContext();
            dummyHttpContext.Request.RequestContext.RouteData.Values.Add("widgetName", "Dummy");
            dummyHttpContext.Request.RequestContext.RouteData.Values.Add("controller", "Designer");
            var urlHelper = new UrlHelper(dummyHttpContext.Request.RequestContext);

            string result;
            using (var factoryReg = new ControllerFactoryRegion<DummyControllerFactory>())
            {
                factoryReg.Factory.ControllerRegistry["Dummy"] = typeof(DummyController);
                factoryReg.Factory.ControllerRegistry["Designer"] = typeof(DesignerController);

                // Act
                result = urlHelper.WidgetContent("Test/MyScript.js");
            }

            // Assert
            Assert.AreEqual("/Frontend-Assembly/Telerik.Sitefinity.Frontend.TestUtilities/Test/MyScript.js", result);
        }

        /// <summary>
        /// The widget content_ relative path no route data_ throws exception.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether WidgetContent throws exception for relative paths when no RouteData is available.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WidgetContent_RelativePathNoRouteData_ThrowsException()
        {
            // Arrange
            var dummyHttpContext = new DummyHttpContext();
            var urlHelper = new UrlHelper(dummyHttpContext.Request.RequestContext);

            // Act
            urlHelper.WidgetContent("Test/MyScript.js");
        }

        #endregion
    }
}