using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that the LayoutRenderer class is working correctly.
    /// </summary>
    [TestClass]
    public class LayoutRendererTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new HttpContextWrapper(new HttpContext(
               new HttpRequest(null, "http://tempuri.org", null),
               new HttpResponse(null)));

            this.context.Items["CurrentResourcePackage"] = "test";
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.context = null;
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method throws exception when used without existing HttpContext.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateController_WithNullContext_ThrowsException()
        {
            SystemManager.RunWithHttpContext(null, () =>
            {
                var layoutTemplateBuilder = new LayoutRenderer();
                layoutTemplateBuilder.CreateController();
            });
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method instantiate a controller with given type.")]
        public void CreateController_WithDummyContext_CreatesControllerInstance()
        {
            //Arrange
            var layoutTemplateBuilder = new LayoutRenderer();

            //Act
            Controller dummyController = null;
            SystemManager.RunWithHttpContext(this.context, () =>
            {
                dummyController = layoutTemplateBuilder.CreateController();
            });

            //Assert
            this.AssertControllerHasValidContext(dummyController);
            Assert.AreEqual<string>(dummyController.ControllerContext.RouteData.Values["controller"].ToString(), "generic");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method instantiate a controller with given type when rotueData is explicitly provided.")]
        public void CreateController_WithRouteData_CreatesControllerInstance()
        {
            //Arrange
            var layoutTemplateBuilder = new LayoutRenderer();

            //Act
            Controller dummyController = null;
            SystemManager.RunWithHttpContext(context, () =>
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", "dummy");
                dummyController = layoutTemplateBuilder.CreateController(routeData);
            });

            //Assert
            this.AssertControllerHasValidContext(dummyController);
            Assert.AreEqual<string>(dummyController.ControllerContext.RouteData.Values["controller"].ToString(), "dummy");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method returns the correct html.")]
        public void RenderViewToString_DummyController_ReturnsCorrectHtmlString()
        {
            //Arrange
            var layoutTemplateBuilder = new DummyLayoutRenderer();
            Controller dummyController = layoutTemplateBuilder.CreateController();

            Assert.IsNotNull(dummyController);

            //Act
            var htmlString = layoutTemplateBuilder.RenderViewToString(dummyController.ControllerContext, "Test");

            //Assert
            Assert.AreEqual<string>(htmlString, layoutTemplateBuilder.InnerHtmlStringWithoutForm);
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutTemplate method returns proper html with appended form tag.")]
        public void GetLayoutTemplate_WithFormTag_ReturnsCorrectHtmlString()
        {
            //Arrange
            var layoutTemplateBuilder = new DummyLayoutRenderer();

            SystemManager.RunWithHttpContext(this.context, () =>
            {
                //Act
                var htmlString = layoutTemplateBuilder.GetLayoutTemplate("");

                //Assert
                Assert.IsTrue(htmlString.StartsWith(LayoutRendererTests.masterPageDirective));
                Assert.IsTrue(htmlString.Contains(layoutTemplateBuilder.InnerHtmlStringWithForm));
            });
        }

        #region Helper methods

        private void AssertControllerHasValidContext(Controller dummyController)
        {
            Assert.IsNotNull(dummyController);
            Assert.IsTrue(dummyController != null);
            Assert.IsTrue(dummyController.ControllerContext != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values.ContainsKey("controller"));
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values["controller"] != null);
        }

        #endregion 

        #region Private fields and constants

        private HttpContextWrapper context;
        private const string masterPageDirective = "<%@ Master Language=\"C#\"";

        #endregion
    }
}
