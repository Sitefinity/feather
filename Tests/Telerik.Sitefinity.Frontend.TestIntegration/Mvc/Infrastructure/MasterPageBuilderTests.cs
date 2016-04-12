using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.TestUtilities.Data;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
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

                var responseContentDescriptionTagsCount = Regex.Matches(responseContent, descriptionTag).Count;
                Assert.IsTrue(responseContentDescriptionTagsCount == 1, "The response content does not contain description meta tag exactly once.");
            }
        }

        /// <summary>
        /// Checks whether default form tag exist in bootstrap package in preview mode
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks for default aspnetForm tag in preview mode.")]
        public void CreatePage_Bootstrap_CheckFormTagExistPreviewMode()
        {
            Guid pageId = Guid.Empty;
            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            PageTemplateFramework pageTemplateFramework = Pages.Model.PageTemplateFramework.Mvc;
            try
            {
                pageId = this.CreatePage(pageTemplateFramework);
                var placeholder = pageTemplateFramework == PageTemplateFramework.Hybrid ? "Body" : "Contentplaceholder1";
                FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId, "RegistrationController", "Registration Controller Widget", placeholder);
                FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId, "ProfileController", "Profile Controller Widget", placeholder);
                string pageContent = this.GetPageContent(pageId, true);

                var formTag = "<form[\\s].*[\\s]?id=\"aspnetForm\".*>.*";
                var formTagsCount = Regex.Matches(pageContent, formTag, RegexOptions.IgnoreCase).Count;

                Assert.IsFalse(formTagsCount == 1, "Default ASP.Net form tag exist in preview mode");
            }
            finally
            {
                if (!featherActivated)
                {
                    FeatherServerOperations.FeatherModule().ActivateFeather();
                }

                if (pageId != Guid.Empty)
                {
                    ServerOperations.Pages().DeletePage(pageId);
                }
            }
        }

        #region Helper methods

        private string GetPageContent(Guid pageId, bool preview)
        {
            PageManager pageManager = PageManager.GetManager();

            var page = pageManager.GetPageNode(pageId);
            var url = page.GetUrl();
            var pageUrl = UrlPath.ResolveUrl(url, true, true);

            if (preview)
            {
                pageUrl += "/action/preview";
            }

            string pageContent = this.GetContent(pageUrl);

            return pageContent;
        }

        private Guid CreatePage(PageTemplateFramework framework)
        {
            Guid pageId = Guid.Empty;
            var suffix = Guid.NewGuid().ToString("N");

            if (framework == PageTemplateFramework.Hybrid)
            {
                var namePrefix = "TestPageName";
                var titlePrefix = "TestPageTitle";
                var urlPrefix = "test-page-url";
                var index = 1;

                pageId = new PageContentGenerator().CreatePage(
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", namePrefix + suffix, index.ToString(CultureInfo.InvariantCulture)),
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", titlePrefix + suffix, index.ToString(CultureInfo.InvariantCulture)),
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", urlPrefix + suffix, index.ToString(CultureInfo.InvariantCulture)));
            }
            else if (framework == PageTemplateFramework.Mvc)
            {
                var pagesOperations = FeatherServerOperations.Pages();
                var pageManager = PageManager.GetManager();

                var bootstrapTemplate = pageManager.GetTemplates().FirstOrDefault(t => (t.Name == "Bootstrap.default" && t.Title == "default") || t.Title == "Bootstrap.default");
                if (bootstrapTemplate == null)
                    throw new ArgumentException("Bootstrap template not found");

                pageId = pagesOperations.CreatePageWithTemplate(bootstrapTemplate, "FormsPageBootstrap" + suffix, "FormsPageBootstrap");
            }

            return pageId;
        }

        private string GetContent(string url)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = 120 * 1000;
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];
            webRequest.CachePolicy = new RequestCachePolicy();
            var webResponse = (HttpWebResponse)webRequest.GetResponse();

            string responseContent;
            using (var sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                responseContent = sr.ReadToEnd();

            return responseContent;
        }

        #endregion
    }
}