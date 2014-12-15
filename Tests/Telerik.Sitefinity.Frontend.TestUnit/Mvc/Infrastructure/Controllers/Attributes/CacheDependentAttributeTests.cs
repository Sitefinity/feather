using System.Web.Mvc;

using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Views;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// The cache dependent attribute tests.
    /// </summary>
    [TestClass]
    public class CacheDependentAttributeTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The on result executed_ virtual file dependencies_ added to response.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether OnResultExecuted will add the cache dependencies of a BuildManagerCompiledView view template virtual file to the response.")]
        public void OnResultExecuted_VirtualFileDependencies_AddedToResponse()
        {
            // Arrange
            var context = new DummyHttpContext();
            context.Items[PageRouteHandler.AddCacheDependencies] = true;

            var response = (DummyHttpResponse)context.Response;
            var filterContext = new ResultExecutedContext();
            filterContext.HttpContext = context;
            var actionFilter = new DummyCacheDependentAttribute();
            var viewPath = "~/MyTestView.cshtml";
            var view = new DummyBuildManagerCompiledView(new ControllerContext(), viewPath);

            filterContext.Result = new DummyViewResult { View = view };

            // Act
            actionFilter.OnResultExecuted(filterContext);

            // Assert
            Assert.AreEqual(1, response.CacheDependencies.Count, "Unexpected number of cache dependencies were added.");
            Assert.IsInstanceOfType(response.CacheDependencies[0], typeof(DummyCacheDependency), "The cache dependency that was added is of unexpected type.");
            
            var dependency = (DummyCacheDependency)response.CacheDependencies[0];
            Assert.AreEqual(viewPath, dependency.Key, "Cache dependency was not added on the expected path.");
        }

        #endregion
    }
}