using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;


namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure
{
    /// <summary>
    /// Ensures that ControllerHelper class works correctly.
    /// </summary>
    [TestClass]
    public class ControllerRegistryTest
    {
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether IsController will return true for our DummyController.")]
        public void IsController_DummyController_ReturnsTrue()
        {
            var result = SitefinityControllerFactroryExtensions.IsController(null, typeof(DummyController));

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether IsController will return false for a System.Web.UI.Control.")]
        public void IsController_DummyControl_ReturnsFalse()
        {
            var result = SitefinityControllerFactroryExtensions.IsController(null, typeof(DummyControl));

            Assert.IsFalse(result);
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetControllerName will throw exception if the type is not specified.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetControllerName_WithoutType_ThrowsException()
        {
            var controllerName = SitefinityControllerFactroryExtensions.GetControllerName(null, null);
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetControllerName returns the controller name by its time as per convention.")]
        public void GetControllerName_WithControllerSuffix_ReturnsNameWithoutSuffix()
        {
            var controllerName = SitefinityControllerFactroryExtensions.GetControllerName(null, typeof(DummyController));

            Assert.AreEqual("Dummy", controllerName);
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetControllerName will return the type name as is if no Controller suffix is present.")]
        public void GetControllerName_WithoutControllerSuffix_ReturnsTypeName()
        {
            var controllerName = SitefinityControllerFactroryExtensions.GetControllerName(null, typeof(DummyControl));

            Assert.AreEqual("DummyControl", controllerName);
        }
    }
}
