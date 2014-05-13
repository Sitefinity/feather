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
            this.objectFactoryCotnainerRegion = new ObjectFactoryContainerRegion();           
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.objectFactoryCotnainerRegion.Dispose();
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
            var context = new HttpContextWrapper(new HttpContext(
               new HttpRequest(null, "http://tempuri.org", null),
               new HttpResponse(null)));
            context.Items["CurrentResourcePackage"] = "test";

            var layoutTemplateBuilder = new LayoutRenderer();

            Controller dummyController = null;
            SystemManager.RunWithHttpContext(context, () =>
            {
                dummyController = layoutTemplateBuilder.CreateController();
            });

            Assert.IsNotNull(dummyController);
            Assert.IsTrue(dummyController != null);
            Assert.IsTrue(dummyController.ControllerContext != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values.ContainsKey("controller"));
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values["controller"] != null);
            Assert.AreEqual<string>(dummyController.ControllerContext.RouteData.Values["controller"].ToString(), "generic");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method instantiate a controller with given type when rotueData is explicitly provided.")]
        public void CreateController_WithRouteData_CreatesControllerInstance()
        {
            var context = new HttpContextWrapper(new HttpContext(
               new HttpRequest(null, "http://tempuri.org", null),
               new HttpResponse(null)));
            context.Items["CurrentResourcePackage"] = "test";

            var layoutTemplateBuilder = new LayoutRenderer();

            Controller dummyController = null;
            SystemManager.RunWithHttpContext(context, () =>
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", "dummy");
                dummyController = layoutTemplateBuilder.CreateController(routeData);
            });

            Assert.IsNotNull(dummyController);
            Assert.IsTrue(dummyController != null);
            Assert.IsTrue(dummyController.ControllerContext != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values != null);
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values.ContainsKey("controller"));
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values["controller"] != null);
            Assert.AreEqual<string>(dummyController.ControllerContext.RouteData.Values["controller"].ToString(), "dummy");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method returns the correct html.")]
        public void RenderViewToString_DummyController_ReturnsCorrectHtmlString()
        {
            var layoutTemplateBuilder = new DummyLayoutRenderer();
            Controller dummyController = layoutTemplateBuilder.CreateController();

            Assert.IsNotNull(dummyController);

            var htmlString = layoutTemplateBuilder.RenderViewToString(dummyController.ControllerContext, "Test");

            Assert.AreEqual<string>(htmlString, layoutTemplateBuilder.InnerHtmlStringWithoutForm);

        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutTemplate method returns proper html with appended form tag.")]
        public void GetLayoutTemplate_WithFormTag_ReturnsCorrectHtmlString()
        {
            var context = new HttpContextWrapper(new HttpContext(
               new HttpRequest(null, "http://tempuri.org", null),
               new HttpResponse(null)));
            context.Items["CurrentResourcePackage"] = "test";

            var layoutTemplateBuilder = new DummyLayoutRenderer();

            SystemManager.RunWithHttpContext(context, () =>
            {
                var htmlString = layoutTemplateBuilder.GetLayoutTemplate("");

                Assert.IsTrue(htmlString.StartsWith(LayoutRendererTests.masterPageDirective));
                Assert.IsTrue(htmlString.Contains(layoutTemplateBuilder.InnerHtmlStringWithForm));
            });
        }

       

        #region Private fields and constants

        private ObjectFactoryContainerRegion objectFactoryCotnainerRegion;
        private const string masterPageDirective = "<%@ Master Language=\"C#\"";

        #endregion
    }
}
