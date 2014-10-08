using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Layouts;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that the LayoutRenderer class is working correctly.
    /// </summary>
    [TestClass]
    public class LayoutRendererTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create controller_ with dummy context_ creates controller instance.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method instantiate a controller with given type.")]
        public void CreateController_WithDummyContext_CreatesControllerInstance()
        {
            // Arrange
            var layoutTemplateBuilder = new LayoutRenderer();

            // Act
            Controller dummyController = null;
            SystemManager.RunWithHttpContext(this.context, () => { dummyController = layoutTemplateBuilder.CreateController(); });

            // Assert
            this.AssertControllerHasValidContext(dummyController);
            Assert.AreEqual(dummyController.ControllerContext.RouteData.Values["controller"].ToString(), "Generic", ignoreCase: true, message: "The controller name is not added in the RouteData collection.");
        }

        /// <summary>
        /// The create controller_ with null context_ throws exception.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method throws exception when used without existing HttpContext.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateController_WithNullContext_ThrowsException()
        {
            SystemManager.RunWithHttpContext(
                null, 
                () =>
                    {
                        var layoutTemplateBuilder = new LayoutRenderer();
                        layoutTemplateBuilder.CreateController();
                    });
        }

        /// <summary>
        /// The create controller_ with route data_ creates controller instance.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method instantiate a controller with given type when rotueData is explicitly provided.")]
        public void CreateController_WithRouteData_CreatesControllerInstance()
        {
            // Arrange
            var layoutTemplateBuilder = new LayoutRenderer();

            // Act
            Controller dummyController = null;
            SystemManager.RunWithHttpContext(
                this.context, 
                () =>
                    {
                        var routeData = new RouteData();
                        routeData.Values.Add("controller", "dummy");
                        dummyController = layoutTemplateBuilder.CreateController(routeData);
                    });

            // Assert
            this.AssertControllerHasValidContext(dummyController);
            Assert.AreEqual(dummyController.ControllerContext.RouteData.Values["controller"].ToString(), "dummy", "The controller name is not added in the RouteData collection.");
        }

        /// <summary>
        /// The get layout template_ with form tag_ returns correct html string.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutTemplate method returns proper html with appended form tag.")]
        public void GetLayoutTemplate_WithFormTag_ReturnsCorrectHtmlString()
        {
            // Arrange
            var layoutTemplateBuilder = new DummyLayoutRenderer();

            SystemManager.RunWithHttpContext(
                this.context, 
                () =>
                    {
                        // Act
                        var htmlString = layoutTemplateBuilder.GetLayoutTemplate(string.Empty);

                        // Assert
                        Assert.IsTrue(htmlString.StartsWith(MasterPageDirective, StringComparison.Ordinal), "The master page directive is not added correctly.");
                        Assert.IsTrue(htmlString.Contains(layoutTemplateBuilder.InnerHtmlStringWithForm), "The method doesn't return the expected html.");
                    });
        }

        /// <summary>
        /// The render view to string_ dummy controller_ returns correct html string.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the method returns the correct html.")]
        public void RenderViewToString_DummyController_ReturnsCorrectHtmlString()
        {
            // Arrange
            var layoutTemplateBuilder = new DummyLayoutRenderer();
            var dummyController = layoutTemplateBuilder.CreateController();

            Assert.IsNotNull(dummyController);

            // Act
            var htmlString = layoutTemplateBuilder.RenderViewToString(dummyController.ControllerContext, "Test");

            // Assert
            Assert.AreEqual(htmlString, layoutTemplateBuilder.InnerHtmlStringWithoutForm, "RenderViewToString method doesn't render the expected html.");
        }

        /// <summary>
        /// The test cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.context = null;
        }

        /// <summary>
        /// The test initialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null)));

            this.context.Items["CurrentResourcePackage"] = "test";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asserts whether the controller has valid context.
        /// </summary>
        /// <param name="dummyController">
        /// The dummy controller.
        /// </param>
        private void AssertControllerHasValidContext(Controller dummyController)
        {
            Assert.IsNotNull(dummyController, "The controller is not created correctly.");
            Assert.IsNotNull(dummyController.ControllerContext, "The ControllerContext is null.");
            Assert.IsNotNull(dummyController.ControllerContext.RouteData, "The RouteData is null.");
            Assert.IsNotNull(dummyController.ControllerContext.RouteData.Values, "The RouteData contains no values.");
            Assert.IsTrue(dummyController.ControllerContext.RouteData.Values.ContainsKey("controller"), "The route data doesn't contain 'controller' value.");
            Assert.IsNotNull(dummyController.ControllerContext.RouteData.Values["controller"], "The value for the 'controller' in the RouteData is null.");
        }

        #endregion

        #region Constants & Fields

        private const string MasterPageDirective = "<%@ Master Language=\"C#\"";
        private HttpContextWrapper context;

        #endregion
    }
}