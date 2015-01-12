using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Infrastructure
{
    /// <summary>
    /// This class contains tests for controller action invoking.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for controller action invoking.")]
    public class DynamicUrlActionInvokerTests
    {
        /// <summary>
        /// Checks whether error message is rendered when a controller throws exception.
        /// </summary>
        [Test]
        [Author("Boyko-Karadzhov")]
        [Description("Checks whether error message is rendered when a controller throws exception.")]
        public void CreatePageWithFailingWidget_RenderPage_ResponseContainsErrorMessage()
        {
            string testName = System.Reflection.MethodInfo.GetCurrentMethod().Name;
            string pageNamePrefix = testName + "MvcPage";
            string pageTitlePrefix = testName + "Mvc Page";
            string urlNamePrefix = testName + "mvc-page";
            int index = 1;
            string url = UrlPath.ResolveAbsoluteUrl("~/" + urlNamePrefix + index);

            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = typeof(DummyFailingController).FullName;

            using (var contentGenerator = new PageContentGenerator())
            {
                contentGenerator.CreatePageWithWidget(mvcProxy, string.Empty, pageNamePrefix, pageTitlePrefix, urlNamePrefix, index);

                string responseContent = WebRequestHelper.GetPageWebContent(url);
                string expectedResult = Res.Get<InfrastructureResources>().ErrorExecutingController;

                Assert.Contains(responseContent, expectedResult, "The expected error message was not found on the page!");
            }
        }
    }
}
