using System.Linq;
using System.Web.Mvc;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Ensures that FrontendControllerFactory class is working correctly.
    /// </summary>
    [TestClass]
    public class FrontendControllerFactoryTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create controller_ decorated with view enhance view engines attribute_ new controllers view engines have custom transformations.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether CreateController will return a new controller with customized search locations when controller is decorated with EnhanceViewEnginesAttribute.")]
        public void CreateController_DecoratedWithViewEnhanceViewEnginesAttribute_NewControllersViewEnginesHaveCustomTransformations()
        {
            // Arrange
            var controllerFactory = new FrontendControllerFactory();
            controllerFactory.RegisterController(typeof(DummyEnhancedController).Name, typeof(DummyEnhancedController));

            var viewEngine = new RazorViewEngine();
            ViewEngines.Engines.Add(viewEngine);
            try
            {
                // Act
                var controller = (Controller)controllerFactory.CreateController(new DummyHttpContext().Request.RequestContext, "DummyEnhanced");

                // Assert
                RazorViewEngine controllerVe = controller.ViewEngineCollection.OfType<RazorViewEngine>().FirstOrDefault();
                Assert.IsNotNull(controllerVe, "The newly created controller does not have the expected view engine.");

                var viewLocationExists = controllerVe.ViewLocationFormats.Any(p => p.StartsWith("~/" + DummyEnhancedController.CustomControllerPath, System.StringComparison.Ordinal));

                Assert.IsTrue(viewLocationExists, "The newly created controller does not have its custom path in the view locations.");
            }
            finally
            {
                ViewEngines.Engines.Remove(viewEngine);
            }
        }

        /// <summary>
        /// The create controller_ dummy controller_ new controller view engines have additinal search paths.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether CreateController will return a new controller with updated view engines collection.")]
        public void CreateController_DummyController_NewControllerViewEnginesHaveAdditionalSearchPaths()
        {
            // Arrange
            var controllerFactory = new FrontendControllerFactory();
            controllerFactory.RegisterController(typeof(DummyController).Name, typeof(DummyController));

            var viewEngine = new RazorViewEngine();
            ViewEngines.Engines.Add(viewEngine);

            try
            {
                // Act
                var controller = (Controller)controllerFactory.CreateController(new DummyHttpContext().Request.RequestContext, "Dummy");

                // Assert
                var controllerVe = controller.ViewEngineCollection.OfType<RazorViewEngine>().FirstOrDefault();
                Assert.IsNotNull(controllerVe, "The newly created controller does not have the expected view engine.");

                var containerVp = string.Format(System.Globalization.CultureInfo.InvariantCulture, "~/{0}", FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(DummyController).Assembly));

                Assert.IsTrue(controllerVe.ViewLocationFormats.Any(v => v.StartsWith(containerVp, System.StringComparison.Ordinal)), "The newly created controller does not have its container path in the view locations.");
            }
            finally
            {
                ViewEngines.Engines.Remove(viewEngine);
            }
        }

        #endregion
    }
}