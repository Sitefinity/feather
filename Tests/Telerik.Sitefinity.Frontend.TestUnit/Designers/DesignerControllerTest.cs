using System;
using System.Web.Mvc;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.Security;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUnit.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Authentication;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Designers
{
    /// <summary>
    /// These tests are meant to ensure that the DesignerController class is working correctly.
    /// </summary>
    [TestClass]
    public class DesignerControllerTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The master_ by widget name_ sets control name.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether Master action sets the correct ControlName in the ViewBag.")]
        public void Master_ByWidgetName_SetsControlName()
        {
            // Arrange
            string widgetName = "Dummy";
            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategyMock>();

                // Act
                var designer = this.designerController.Master(widgetName) as ViewResult;

                // Assert
                Assert.AreEqual(widgetName, designer.ViewBag.ControlName, string.Format(System.Globalization.CultureInfo.InvariantCulture, "ViewBag.ControlName should be equal to {0}.", widgetName));
            }
        }

        /// <summary>
        /// The test cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            FrontendManager.AuthenticationEvaluator = new AuthenticationEvaluator();

            this.objectFactoryCotnainerRegion.Dispose();
            this.designerController.Dispose();
            this.designerController = null;
        }

        /// <summary>
        /// The test initialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.objectFactoryCotnainerRegion = new ObjectFactoryContainerRegion();

            FrontendManager.AuthenticationEvaluator = new DummyAuthenticationEvaluator { IsBackendUser = true };

            this.designerController = new DummyDesignerController();
        }

        /// <summary>
        /// The view_ no local property grid designer_ returns default property grid designer.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether View action returns default view with the requested view type when such is available.")]
        public void View_NoLocalPropertyGridDesigner_ReturnsDefaultPropertyGridDesigner()
        {
            // Arrange
            var viewType = "PropertyGrid";
            var expectedDesignerViewName = "DesignerView.PropertyGrid";

            // Act
            var designerView = this.designerController.View(string.Empty, viewType) as PartialViewResult;

            // Assert
            Assert.AreEqual(expectedDesignerViewName, designerView.ViewName, string.Format(System.Globalization.CultureInfo.InvariantCulture, "ViewName should be equal to {0}.", expectedDesignerViewName));
        }

        /// <summary>
        /// The view_ with local advanced designer_ returns property grid designer.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Validates whether View action returns view with correct name with requested view type when such is available.")]
        public void View_WithLocalAdvancedDesigner_ReturnsPropertyGridDesigner()
        {
            // Arrange
            var widgetName = "Dummy";
            var viewType = "PropertyGrid";
            var expectedDesignerViewName = "DesignerView.PropertyGrid";

            // Act
            var designerView = this.designerController.View(widgetName, viewType) as PartialViewResult;

            // Assert
            Assert.AreEqual(expectedDesignerViewName, designerView.ViewName, string.Format(System.Globalization.CultureInfo.InvariantCulture, "ViewName should be equal to {0}.", expectedDesignerViewName));
        }

        #endregion

        #region Fields

        private DesignerController designerController;
        private ObjectFactoryContainerRegion objectFactoryCotnainerRegion;

        #endregion
    }
}