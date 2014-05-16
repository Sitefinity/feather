using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;


namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Ensures that FrontendControllerFactory class is working correctly.
    /// </summary>
    [TestClass]
    public class FrontendControllerFactoryTests
    {
        #region CreateController

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether CreateController will return a new controller with updated view engines collection.")]
        public void CreateController_DummyController_NewControllerViewEnginesHaveAdditinalSearchPaths()
        {
            //Arrange
            var controllerFactory = new FrontendControllerFactory();
            controllerFactory.RegisterController(typeof(DummyController).Name, typeof(DummyController));

            var viewEngine = new RazorViewEngine();
            ViewEngines.Engines.Add(viewEngine);
            try
            {
                //Act
                var controller = (Controller)controllerFactory.CreateController(new DummyHttpContext().Request.RequestContext, "Dummy");
                
                //Assert
                var controllerVe = controller.ViewEngineCollection.OfType<RazorViewEngine>().FirstOrDefault();
                Assert.IsNotNull(controllerVe, "The newly created controller does not have the expected view engine.");

                var containerVp = "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(DummyController).Assembly);
                Assert.IsTrue(controllerVe.ViewLocationFormats.Any(v => v.StartsWith(containerVp)),
                    "The newly created controller does not have its container path in the view locations.");
            }
            finally
            {
                ViewEngines.Engines.Remove(viewEngine);
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether CreateController will return a new controller with customized search locations when controller is decorated with EnhanceViewEnginesAttribute.")]
        public void CreateController_DecoratedWithViewEnhanceViewEnginesAttribute_NewControllersViewEnginesHaveCustomTransformations()
        {
            //Arrange
            var controllerFactory = new FrontendControllerFactory();
            controllerFactory.RegisterController(typeof(DummyEnhancedController).Name, typeof(DummyEnhancedController));

            var viewEngine = new RazorViewEngine();
            ViewEngines.Engines.Add(viewEngine);
            try
            {
                //Act
                var controller = (Controller)controllerFactory.CreateController(new DummyHttpContext().Request.RequestContext, "DummyEnhanced");
                
                //Assert
                var controllerVe = controller.ViewEngineCollection.OfType<RazorViewEngine>().FirstOrDefault();
                Assert.IsNotNull(controllerVe, "The newly created controller does not have the expected view engine.");

                Assert.IsTrue(controllerVe.ViewLocationFormats.Any(p => p.StartsWith("~/" + DummyEnhancedController.CustomControllerPath)),
                    "The newly created controller does not have its custom path in the view locations.");
            }
            finally
            {
                ViewEngines.Engines.Remove(viewEngine);
            }
        }

        #endregion
    }
}
