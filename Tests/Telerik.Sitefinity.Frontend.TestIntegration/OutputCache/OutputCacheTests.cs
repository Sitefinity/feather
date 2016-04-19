using System;
using System.Linq;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.OutputCache
{
    [TestFixture]
    [Description("This is a class with tests related to output cache functionality of feather widgets.")]
    public class OutputCacheTests
    {
        /// <summary>
        /// Checks whether page with MVC widget is cached properly.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings"), Test]
        [Category(TestCategories.MvcCore)]
        [Author(FeatherTeams.SitefinityTeam2)]
        [Description("Checks whether page with MVC widget is cached properly.")]
        public void OutputCache_NoTempData_IsRetrievedFromCache()
        {
            var pageTitle = "FeatherPage";
            var pageUrl = "featherpage";
            var widgetName = "DateTimeWidet";
            var placeholder = "Contentplaceholder1";
            var url = UrlPath.ResolveAbsoluteUrl("~/" + pageUrl);

            try
            {
                DateTimeController.Count = 0;
                PageManager pageManager = PageManager.GetManager();
                var template = pageManager.GetTemplates().FirstOrDefault(t => (t.Name == "Bootstrap.default" && t.Title == "default") || t.Title == "Bootstrap.default");
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, pageTitle, pageUrl);
                FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId, typeof(DateTimeController).FullName, widgetName, placeholder);

                using (new AuthenticateUserRegion(null))
                {
                    var webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                    webRequest.GetResponse();
                    webRequest.GetResponse();

                    Assert.AreEqual(1, DateTimeController.Count, "The content should be the same because the page is cached.");
                }
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
            }
        }
    }
}
