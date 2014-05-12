using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Security;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;

namespace Telerik.Sitefinity.Frontend.Test.Designers
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

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether GetDesigner action returns view with correct name.")]
        public void GetDesigner_ByWidgetName_ReturnsDesigner()
        {
            //Arrange
            var widgetName = "Dummy";
            var expectedDesignerName = "Master.Designer";

            //Act
            var designerView = this.designerController.GetDesigner(widgetName) as ViewResult;

            //Assert
            Assert.AreEqual(expectedDesignerName, designerView.ViewName, string.Format("ViewName should equals {0}.", expectedDesignerName));
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether GetDesigner action returns default view when the widget name is not provided.")]
        public void GetDesigner_NoLocalDesigner_ReturnsDefaultDesigner()
        {
            //Arrange
            var expectedDesignerName = "Master.Designer";

            //Act
            var designerView = this.designerController.GetDesigner("") as ViewResult;

            //Assert
            Assert.AreEqual(expectedDesignerName, designerView.ViewName, string.Format("ViewName should equals {0}.", expectedDesignerName));
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether GetDesignerPartialView action returns view with correct name with requested view type when such is available.")]
        public void GetDesignerPartialView_WithLocalAdvancedDesigner_ReturnsAdvancedDesigner()
        {
            //Arrange
            var widgetName = "Dummy";
            var viewType = "Advanced";
            var expectedDesignerViewName = "Advanced.Designer";

            //Act
            var designerView = this.designerController.GetDesignerPartialView(widgetName, viewType) as PartialViewResult;

            //Assert
            Assert.AreEqual(expectedDesignerViewName, designerView.ViewName, string.Format("ViewName should equals {0}.", expectedDesignerViewName));
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether GetDesignerPartialView action returns default view with the requested view type when such is available.")]
        public void GetDesignerPartialView_NoLocalAdvancedDesigner_ReturnsDefaultAdvancedDesigner()
        {
            //Arrange
            var viewType = "Advanced";
            var expectedDesignerViewName = "Advanced.Designer";

            //Act
            var designerView = this.designerController.GetDesignerPartialView("", viewType) as PartialViewResult;

            //Assert
            Assert.AreEqual(expectedDesignerViewName, designerView.ViewName, string.Format("ViewName should equals {0}.", expectedDesignerViewName));
        }

        #region Private fields and constants

        private DesignerController designerController;
        private ObjectFactoryContainerRegion objectFactoryCotnainerRegion;
        private Type authenticationEvaluatorType = typeof(AuthenticationEvaluator); 

        #endregion

    }
}