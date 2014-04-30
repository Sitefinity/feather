using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.Test.DummyClasses;
using Telerik.Sitefinity.Frontend.Test.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;

namespace Telerik.Sitefinity.Frontend.Test.Resources
{
    [TestClass]
    public class VirtualPathBuilderTest
    {
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetVirtualPath returns the expected path from a given assembly.")]
        public void GetVirtualPath_TestAssembly_ReturnsAssemblyNameWithPrefix()
        {
            var controllerAssembly = Assembly.GetExecutingAssembly();
            var result = new VirtualPathBuilder().GetVirtualPath(controllerAssembly);

            Assert.AreEqual("Frontend-Assembly/Telerik.Sitefinity.Frontend.Test/", result);
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetVirtualPath for a registered controller will return the virtual path for its assembly.")]
        public void GetVirtualPath_RegisteredWidget_ReturnsVirtualPathForItsAssembly()
        {
            var controllerAssembly = typeof(DummyController).Assembly;
            var vpBuilder = new VirtualPathBuilder();
            var expected = vpBuilder.GetVirtualPath(controllerAssembly);

            using (var factoryReg = new ControllerFactoryRegion<DummyControllerFactory>())
            {
                factoryReg.Factory.ControllerRegistry["Dummy"] = typeof(DummyController);

                var result = vpBuilder.GetVirtualPath(typeof(DummyController));
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if GetVirtualPath throws ArgumentNullException when controllerName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetVirtualPath_PassNull_ThrowsArgumentNullException()
        {
            new VirtualPathBuilder().GetVirtualPath(type: null);
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if GetVirtualPath throws ArgumentNullException when null is passed as argument.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCVirtualPath_NullAssembly_ThrowsArgumentNullException()
        {
            new VirtualPathBuilder().GetVirtualPath(assembly: null);
        }
    }
}
