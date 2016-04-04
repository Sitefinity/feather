using System.Reflection;
using System.Web.Mvc;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Routing
{
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This is a class with tests related to routing in MVC.")]
    public class RoutingTests
    {
        /// <summary>
        /// Ensures that when two MVC widgets are on the page the page can be requested successfully when the second widget does not resolve the URL.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that when two MVC widgets are on the page the page can be requested successfully when the second widget does not resolve the URL.")]
        public void RequestDetailsPage_WithNewsWidgetAndContentBlock_ResponseOk()
        {
            using (var contentGenerator = new PageContentGenerator())
            {
                var testName = MethodInfo.GetCurrentMethod().Name;
                var pageNamePrefix = testName + "MvcPage";
                var pageTitlePrefix = testName + "Mvc Page";
                var urlNamePrefix = testName + "mvc-page";
                var newsItemName = testName + "-news";
                var index = 1;

                var mvcProxy = new MvcControllerProxy();
                mvcProxy.ControllerName = typeof(RoutingTests.NewsController).FullName;

                var pageId = contentGenerator.CreatePageWithWidget(mvcProxy, string.Empty, pageNamePrefix, pageTitlePrefix, urlNamePrefix, 1);

                mvcProxy = new MvcControllerProxy();
                mvcProxy.ControllerName = typeof(RoutingTests.ContentBlockController).FullName;

                PageContentGenerator.AddControlToPage(pageId, mvcProxy, typeof(RoutingTests.ContentBlockController).Name);

                try
                {
                    var newsItemId = ServerOperations.News().CreatePublishedNewsItem(newsItemName);
                    var newsItemUrl = App.WorkWith().NewsItem(newsItemId).Get().ItemDefaultUrl;

                    string url = UrlPath.ResolveAbsoluteUrl("~/" + urlNamePrefix + index + "/" + newsItemUrl);
                    
                    Assert.DoesNotThrow(() => WebRequestHelper.GetPageWebContent(url), "Could not get the page successfully.");
                }
                finally
                {
                    ServerOperations.News().DeleteNewsItem(newsItemName);
                }
            }
        }

        /// <summary>
        /// Ensures that attribute routing with relative routes on controller actions works.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that attribute routing with relative routes on controller actions works.")]
        public void RequestPage_WithAttributeRoutedWidget_RoutedCorrectly()
        {
            using (var contentGenerator = new PageContentGenerator())
            {
                var testName = MethodInfo.GetCurrentMethod().Name;
                var pageNamePrefix = testName + "MvcPage";
                var pageTitlePrefix = testName + "Mvc Page";
                var urlNamePrefix = testName + "mvc-page";
                var index = 1;

                var mvcProxy = new MvcControllerProxy();
                mvcProxy.ControllerName = typeof(AttributeRoutingTestController).FullName;

                contentGenerator.CreatePageWithWidget(mvcProxy, string.Empty, pageNamePrefix, pageTitlePrefix, urlNamePrefix, 1);

                string url = UrlPath.ResolveAbsoluteUrl("~/" + urlNamePrefix + index + "/" + AttributeRoutingTestController.RoutePrefix + "/" + AttributeRoutingTestController.RelativeRoute);
                string content = string.Empty;

                Assert.DoesNotThrow(() => content = WebRequestHelper.GetPageWebContent(url), "Could not get the page successfully.");
                Assert.Contains(content, AttributeRoutingTestController.Content, "The correct action was not rendered.");
            }
        }

        private class NewsController : Controller
        {
            public ActionResult Details(NewsItem item)
            {
                return this.Content("Detail item is resolved. " + item.UrlName);
            }
        }

        private class ContentBlockController : Controller
        {
            public ActionResult Index()
            {
                return this.Content("Content block");
            }

            protected override void HandleUnknownAction(string actionName)
            {
                this.Index().ExecuteResult(this.ControllerContext);
            }
        }
    }
}
