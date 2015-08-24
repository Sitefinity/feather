using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.TestUtilities.Data;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Infrastructure
{
    /// <summary>
    /// This class contains tests for master page builder.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for master page builder.")]
    public class MasterPageBuilderTests
    {
        /// <summary>
        /// Checks whether description is added just once in page markup.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether description is added just once in page markup.")]
        public void CreatePageWithDescription_RenderPage_CheckDescription()
        {
            var testName = System.Reflection.MethodInfo.GetCurrentMethod().Name;
            var pageName = testName + "MvcPage";
            var pageTitle = testName + "Mvc Page";
            var urlName = testName + "mvc-page";
            var description = "customdescription1";
            var descriptionTag = "<meta name=\"description\" content=\"customdescription1\" />";
            string url = UrlPath.ResolveAbsoluteUrl("~/" + urlName);

            using (var contentGenerator = new PageContentGenerator())
            {
                var pageId = contentGenerator.CreatePage(pageName, pageTitle, urlName);
                var pageManager = PageManager.GetManager();
                var page = pageManager.GetPageNode(pageId);
                page.Description = description;
                page.GetPageData().Description = description;
                pageManager.SaveChanges();

                string responseContent = WebRequestHelper.GetPageWebContent(url);

                Assert.IsTrue(responseContent.Contains(descriptionTag), "The response content doesn't contain description meta tag.");

                var contentWithoutDescr = responseContent.Replace(descriptionTag, string.Empty);
                Assert.IsFalse(contentWithoutDescr.Contains(descriptionTag), "The response content contains description meta tag more than once.");
            }
        }
    }
}
