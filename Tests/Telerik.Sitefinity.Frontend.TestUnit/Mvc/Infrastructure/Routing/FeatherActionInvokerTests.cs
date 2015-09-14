using System.Web.Mvc;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Routing;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Ensures that the FeatherActionInvoker class is working correctly.
    /// </summary>
    [TestClass]
    public class FeatherActionInvokerTests
    {
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetDefaultParamsMapper will return null when no mapper should be added based on the current conventions.")]
        public void GetDefaultParamsMapper_NoMatchingConventions_ReturnsNull()
        {
            // Arrange
            var controller = new DummyController();
            controller.ControllerContext = new ControllerContext();
            var actionInvoker = new FeatherActionInvokerMock();

            // Act
            var result = actionInvoker.GetDefaultParamsMapperPublic(controller);

            // Assert
            Assert.IsNull(result, "Resolved mapper when none should have been resolved.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetDefaultParamsMapper will return a chain with mappers in the correct order when all conventions are met.")]
        public void GetDefaultParamsMapper_AllConventionsMet_ReturnsMappersInCorrectOrder()
        {
            // Arrange
            var controller = new DummyMasterDetailController();
            controller.ControllerContext = new ControllerContext();
            var actionInvoker = new FeatherActionInvokerMock();

            // Act
            var result = actionInvoker.GetDefaultParamsMapperPublic(controller);

            // Assert
            Assert.IsNotNull(result, "GetDefaultParamsMapper did not resolve any mappers.");
            Assert.IsInstanceOfType(result, typeof(DetailActionParamsMapper), "GetDefaultParamsMapper did not return the mappers in the expected order.");

            var taxonPagedMapper = result.Next;
            Assert.IsNotNull(taxonPagedMapper, "GetDefaultParamsMapper returned less than the expected number of mappers.");
            Assert.IsInstanceOfType(taxonPagedMapper, typeof(CustomActionParamsMapper), "GetDefaultParamsMapper did not return the mappers in the expected order.");

            var taxonMapper = taxonPagedMapper.Next;
            Assert.IsNotNull(taxonMapper, "GetDefaultParamsMapper returned less than the expected number of mappers.");
            Assert.IsInstanceOfType(taxonMapper, typeof(CustomActionParamsMapper), "GetDefaultParamsMapper did not return the mappers in the expected order.");

            var classificationMapper = taxonMapper.Next;
            Assert.IsNotNull(classificationMapper, "GetDefaultParamsMapper returned less than the expected number of mappers.");
            Assert.IsInstanceOfType(classificationMapper, typeof(TaxonomyUrlParamsMapper), "GetDefaultParamsMapper did not return the mappers in the expected order.");

            var pagingMapper = classificationMapper.Next;
            Assert.IsNotNull(pagingMapper, "GetDefaultParamsMapper returned less than the expected number of mappers.");
            Assert.IsInstanceOfType(pagingMapper, typeof(CustomActionParamsMapper), "GetDefaultParamsMapper did not return the mappers in the expected order.");

            var defaultMapper = pagingMapper.Next;
            Assert.IsNotNull(defaultMapper, "GetDefaultParamsMapper returned less than the expected number of mappers.");
            Assert.IsInstanceOfType(defaultMapper, typeof(DefaultUrlParamsMapper), "GetDefaultParamsMapper did not return the mappers in the expected order.");
        }
    }
}
