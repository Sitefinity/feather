using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Views;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Controllers.Attributes
{
    [TestClass]
    public class CacheDependentAttributeTests
    {
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether OnResultExecuted will add the cache dependencies of a BuildManagerCompiledView view template virtual file to the response.")]
        public void OnResultExecuted_VirtualFileDependencies_AddedToResponse()
        {
            //Arrange
            var context = new DummyHttpContext();
            var response = (DummyHttpResponse)context.Response;
            var filterContext = new ResultExecutedContext();
            var actionFilter = new DummyCacheDependentAttribute();
            var viewPath = "~/MyTestView.cshtml";
            var view = new DummyBuildManagerCompiledView(new ControllerContext(), viewPath);
            filterContext.Result = new DummyViewResult() { View = view };

            //Act
            SystemManager.RunWithHttpContext(context, () =>
                {
                    actionFilter.OnResultExecuted(filterContext);
                });

            //Assert
            Assert.AreEqual(1, response.CacheDependencies.Count, "Unexpected number of cache dependencies were added.");
            Assert.IsInstanceOfType(response.CacheDependencies[0], typeof(DummyCacheDependency), "The cache dependency that was added is of unexpected type.");
            var dependency = (DummyCacheDependency)response.CacheDependencies[0];
            Assert.AreEqual(viewPath, dependency.Key, "Cache dependency was not added on the expected path.");
        }
    }
}
