using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Controls;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;


namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Ensures that SitefinityControllerFactroryExtensions class works correctly.
    /// </summary>
    [TestClass]
    public class SitefinityControllerFactroryExtensionsTests
    {
        #region IsController 

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether IsController will return true for our DummyController.")]
        public void IsController_DummyController_ReturnsTrue()
        {
            //Act
            var result = SitefinityControllerFactroryExtensions.IsController(null, typeof(DummyController));

            //Assert
            Assert.IsTrue(result, "The method fail to recognize a controller.");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether IsController will return false for a System.Web.UI.Control.")]
        public void IsController_DummyControl_ReturnsFalse()
        {
            //Act
            var result = SitefinityControllerFactroryExtensions.IsController(null, typeof(DummyControl));

            //Assert
            Assert.IsFalse(result, "The method recognize simple Control as Controller.");
        }

        #endregion 

        #region GetControllerName

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetControllerName will throw exception if the type is not specified.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetControllerName_WithoutType_ThrowsException()
        {
            //Act
            var controllerName = SitefinityControllerFactroryExtensions.GetControllerName(null, null);
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetControllerName returns the controller name by its time as per convention.")]
        public void GetControllerName_WithControllerSuffix_ReturnsNameWithoutSuffix()
        {
            //Act
            var controllerName = SitefinityControllerFactroryExtensions.GetControllerName(null, typeof(DummyController));

            //Assert
            Assert.AreEqual("Dummy", controllerName, "Controller name is not retrieved correctly.");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetControllerName will return the type name as is if no Controller suffix is present.")]
        public void GetControllerName_WithoutControllerSuffix_ReturnsTypeName()
        {
            //Act
            var controllerName = SitefinityControllerFactroryExtensions.GetControllerName(null, typeof(DummyControl));

            //Assert
            Assert.AreEqual("DummyControl", controllerName, "The controller name is not retrieved correctly.");
        }

        #endregion
    }
}
