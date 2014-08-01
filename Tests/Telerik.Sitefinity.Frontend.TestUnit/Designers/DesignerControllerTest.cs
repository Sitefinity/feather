using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.Security;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Authentication;

namespace Telerik.Sitefinity.Frontend.TestUnit.Designers
{
    /// <summary>
    /// These tests are meant to ensure that the DesignerController class is working correctly.
    /// </summary>
    [TestClass]
    public class DesignerControllerTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            this.objectFactoryCotnainerRegion = new ObjectFactoryContainerRegion();

            FrontendManager.AuthenticationEvaluator = new DummyAuthenticationEvaluator() { IsBackendUser = true };
            
            this.designerController = new DesignerController();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            FrontendManager.AuthenticationEvaluator = new AuthenticationEvaluator();

            this.objectFactoryCotnainerRegion.Dispose();
            this.designerController.Dispose();
            this.designerController = null;
        }

        #region Master

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether Master action sets the correct ControlName in the ViewBag.")]
        public void Master_ByWidgetName_SetsControlName()
        {
            //Arrange
            var widgetName = "Dummy";
            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>();

                //Act
                var designer = this.designerController.Master(widgetName) as ViewResult;

                //Assert
                Assert.AreEqual(widgetName, designer.ViewBag.ControlName, string.Format("ViewBag.ControlName should be equal to {0}.", widgetName));
            }
        }

        #endregion

        #region DesignerPartialView

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether View action returns view with correct name with requested view type when such is available.")]
        public void View_WithLocalAdvancedDesigner_ReturnsPropertyGridDesigner()
        {
            //Arrange
            var widgetName = "Dummy";
            var viewType = "PropertyGrid";
            var expectedDesignerViewName = "DesignerView.PropertyGrid";

            //Act
            var designerView = this.designerController.View(widgetName, viewType) as PartialViewResult;

            //Assert
            Assert.AreEqual(expectedDesignerViewName, designerView.ViewName, string.Format("ViewName should be equal to {0}.", expectedDesignerViewName));
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether View action returns default view with the requested view type when such is available.")]
        public void View_NoLocalPropertyGridDesigner_ReturnsDefaultPropertyGridDesigner()
        {
            //Arrange
            var viewType = "PropertyGrid";
            var expectedDesignerViewName = "DesignerView.PropertyGrid";

            //Act
            var designerView = this.designerController.View("", viewType) as PartialViewResult;

            //Assert
            Assert.AreEqual(expectedDesignerViewName, designerView.ViewName, string.Format("ViewName should be equal to {0}.", expectedDesignerViewName));
        }

        #endregion

        #region Private fields and constants

        private DesignerController designerController;
        private ObjectFactoryContainerRegion objectFactoryCotnainerRegion;

        #endregion

    }
}