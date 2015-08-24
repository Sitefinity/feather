using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// The controller extensions tests.
    /// </summary>
    [TestClass]
    public class ControllerExtensionsTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// Checks if AddCacheDependencies adds the given dependencies and keeps the existing keys.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if AddCacheDependencies adds the given dependencies and keeps the existing keys.")]
        public void AddCacheDependencies_HasCacheItem_CreatesAndAddsNewList()
        {
            // Arrange
            var context = new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://tempuri.org/", null), new HttpResponse(null)));
            var prevDependencies = new List<CacheDependencyKey>(2);
            var prevKey1 = new CacheDependencyKey { Key = "prevKey1", Type = this.GetType() };
            var prevKey2 = new CacheDependencyKey { Key = "prevKey2", Type = this.GetType() };

            prevDependencies.Add(prevKey1);
            prevDependencies.Add(prevKey2);
            context.Items[PageCacheDependencyKeys.PageData] = prevDependencies;

            var controller = new DesignerController();
            var dependencies = new List<CacheDependencyKey>(2);
            var depKey1 = new CacheDependencyKey { Key = "mykey1", Type = this.GetType() };
            var depKey2 = new CacheDependencyKey { Key = "mykey2", Type = this.GetType() };

            dependencies.Add(depKey1);
            dependencies.Add(depKey2);

            // Act
            SystemManager.RunWithHttpContext(context, () => { controller.AddCacheDependencies(dependencies); });

            // Assert
            object cacheObject = context.Items[PageCacheDependencyKeys.PageData];
            Assert.IsNotNull(cacheObject, "No cache object was set in the context.");
            Assert.IsTrue(cacheObject is IList<CacheDependencyKey>, "The cache object was not of the expected type.");

            var cacheList = (IList<CacheDependencyKey>)cacheObject;

            Assert.IsTrue(cacheList.Contains(prevKey1), "The first previous cache dependency was not found.");
            Assert.IsTrue(cacheList.Contains(prevKey2), "The second previous cache dependency was not found.");
            Assert.IsTrue(cacheList.Contains(depKey1), "The first new cache dependency was not added.");
            Assert.IsTrue(cacheList.Contains(depKey2), "The second new cache dependency was not added.");
            Assert.AreEqual(2 + dependencies.Count, cacheList.Count, "There is an unexpecred count of cache dependency keys.");
        }

        /// <summary>
        /// Checks if AddCacheDependencies adds the given dependencies to a new object when none is added before that.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if AddCacheDependencies adds the given dependencies to a new object when none is added before that.")]
        public void AddCacheDependencies_NoCacheItem_CreatesAndAddsNewList()
        {
            // Arrange
            var context = new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://tempuri.org/", null), new HttpResponse(null)));

            var controller = new DesignerController();
            var dependencies = new List<CacheDependencyKey>();

            var depKey1 = new CacheDependencyKey { Key = "mykey1", Type = this.GetType() };
            var depKey2 = new CacheDependencyKey { Key = "mykey2", Type = this.GetType() };

            dependencies.Add(depKey1);
            dependencies.Add(depKey2);

            // Act
            SystemManager.RunWithHttpContext(context, () => controller.AddCacheDependencies(dependencies));

            // Assert
            var cacheObject = context.Items[PageCacheDependencyKeys.PageData];
            Assert.IsNotNull(cacheObject, "No cache object was set in the context.");
            Assert.IsTrue(cacheObject is IList<CacheDependencyKey>, "The cache object was not of the expected type.");

            var cacheList = (IList<CacheDependencyKey>)cacheObject;
            Assert.IsTrue(cacheList.Contains(depKey1), "The first cache dependency was not added.");
            Assert.IsTrue(cacheList.Contains(depKey2), "The second cache dependency was not added.");
            Assert.AreEqual(dependencies.Count, cacheList.Count, "There is an unexpected count of cache dependency keys.");
        }

        /// <summary>
        /// Checks if AddCacheDependencies method throws InvalidOperationException when the current HttpContext is null.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if AddCacheDependencies method throws InvalidOperationException when the current HttpContext is null.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddCacheDependencies_NullContext_ThrowsInvalidOperationException()
        {
            Assert.IsNull(SystemManager.CurrentHttpContext, "Current HttpContext is expected to be null in unit tests but here it is not!");

            Controller controller = new DesignerController();
            controller.AddCacheDependencies(new CacheDependencyKey[0]);
        }

        /// <summary>
        /// Checks if AddCacheDepencies method throws ArgumentNullException when the controller is null.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if AddCacheDepencies method throws ArgumentNullException when the controller is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddCacheDependencies_NullController_ThrowsArgumentNullException()
        {
            Controller controller = null;
            controller.AddCacheDependencies(new CacheDependencyKey[0]);
        }

        /// <summary>
        /// Checks if AddCacheDepencies method throws ArgumentNullException when the keys are null.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if AddCacheDepencies method throws ArgumentNullException when the keys are null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddCacheDependencies_NullKeys_ThrowsArgumentNullException()
        {
            Controller controller = new DesignerController();

            /// TODO: fix always is null expression
            controller.AddCacheDependencies(null);
        }

        /// <summary>
        /// Checks if AddCacheDepencies method throws ArgumentNullException when the keys are null.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if GetIndexRenderMode method throws ArgumentNullException when the controller is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetIndexRenderMode_NullController_ThrowsArgumentNullException()
        {
            Controller controller = null;
            controller.GetIndexRenderMode();
        }

        /// <summary>
        /// Checks if GetIndexRenderMode method returns Normal for controllers without the attribute.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if GetIndexRenderMode method returns Normal for controllers without the attribute.")]
        public void GetIndexRenderMode_NoAttributeController_ReturnsNormal()
        {
            var controller = new DummyController();
            var result = controller.GetIndexRenderMode();

            Assert.AreEqual(IndexRenderModes.Normal, result, "Default value for IndexRenderMode was not correct.");
        }

        /// <summary>
        /// Checks if GetIndexRenderMode method returns NoOutput for controllers with attribute and mode set to NoOutput.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks if GetIndexRenderMode method returns NoOutput for controllers with attribute and mode set to NoOutput.")]
        public void GetIndexRenderMode_AttributeNoOutputController_ReturnsNoOutput()
        {
            var controller = new DummyNoOutputInIndexingController();
            var result = controller.GetIndexRenderMode();

            Assert.AreEqual(IndexRenderModes.NoOutput, result, "IndexRenderMode was not retreived correctly.");
        }

        #endregion
    }
}