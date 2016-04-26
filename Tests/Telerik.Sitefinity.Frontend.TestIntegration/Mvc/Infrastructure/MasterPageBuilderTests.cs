using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
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
        public void CreatePage_Bootstrap_CheckFormTagNotExistPreviewMode()
        {
            Guid bootstrapPageId = Guid.Empty;
            Guid minimalPageId = Guid.Empty;
            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();

            try
            {
                var suffix = Guid.NewGuid().ToString("N");
                var bootstrapTemplate = this.GetDefaultBootstrapTemplate();
                var minimalTemplate = this.GetDefaultMinimalTemplate();
                var bootstrapPageName = "PageBootstrap";
                var minimalPageName = "PageMinimal";
                var previewFormTagPattern = "<form[\\s].*[\\s]?id=\"aspnetForm\".*>.*";

                // page preview bootstrap
                bootstrapPageId = FeatherServerOperations.Pages().CreatePageWithTemplate(bootstrapTemplate, bootstrapPageName + suffix, bootstrapPageName + suffix);
                var bootstrapPagePreviewFormTagsCount = this.PagePreviewFormTags(bootstrapPageId, previewFormTagPattern);

                // template preview bootstrap
                var bootstrapTemplatePreviewFormTagsCount = this.TemplatePreviewFormTags(bootstrapTemplate, previewFormTagPattern);

                // page preview minimal
                minimalPageId = FeatherServerOperations.Pages().CreatePageWithTemplate(minimalTemplate, minimalPageName + suffix, minimalPageName + suffix);
                var minimalPagePreviewFormTagsCount = this.PagePreviewFormTags(minimalPageId, previewFormTagPattern);

                // template preview minimal
                var minimalTemplatePreviewFormTagsCount = this.TemplatePreviewFormTags(minimalTemplate, previewFormTagPattern);

                Assert.IsFalse(bootstrapPagePreviewFormTagsCount == 1, "Default ASP.Net form tag exist in Bootstrap page preview mode");
                Assert.IsFalse(bootstrapTemplatePreviewFormTagsCount == 1, "Default ASP.Net form tag exist in Bootstrap template preview mode");
                Assert.IsFalse(minimalPagePreviewFormTagsCount == 1, "Default ASP.Net form tag exist in Minimal page preview mode");
                Assert.IsFalse(minimalTemplatePreviewFormTagsCount == 1, "Default ASP.Net form tag exist in Minimal template preview mode");
            }
            finally
            {
                if (bootstrapPageId != Guid.Empty)
                {
                    ServerOperations.Pages().DeletePage(bootstrapPageId);
                }

                if (minimalPageId != Guid.Empty)
                {
                    ServerOperations.Pages().DeletePage(minimalPageId);
                }
            }
        }

        private int TemplatePreviewFormTags(PageTemplate bootstrapTemplate, string previewFormTagPattern)
        {
            var bootstrapTemplatePreviewPageUrl = string.Concat(
                HttpContext.Current.Request.Url.Scheme,
                "://",
                HttpContext.Current.Request.Url.Host,
                "/Sitefinity/Template/",
                bootstrapTemplate.Id,
                "/Preview/");
            string bootstrapTemplatePreviewContent = this.GetContent(bootstrapTemplatePreviewPageUrl);
            var bootstrapTemplatePreviewFormTagsCount = Regex.Matches(bootstrapTemplatePreviewContent, previewFormTagPattern, RegexOptions.IgnoreCase).Count;

            return bootstrapTemplatePreviewFormTagsCount;
        }

        private int PagePreviewFormTags(Guid bootstrapPageId, string previewFormTagPattern)
        {
            string bootstrapPagePreviewContent = FeatherServerOperations.Pages().GetPageContent(bootstrapPageId, true, "action/preview");
            var bootstrapPagePreviewFormTagsCount = Regex.Matches(bootstrapPagePreviewContent, previewFormTagPattern, RegexOptions.IgnoreCase).Count;

            return bootstrapPagePreviewFormTagsCount;
        }

        private PageTemplate GetDefaultBootstrapTemplate()
        {
            var pageManager = PageManager.GetManager();

            var bootstrapTemplate = pageManager.GetTemplates().FirstOrDefault(t => (t.Name == LayoutFileManager.BootstrapDefaultTemplateName && t.Title == "default") ||
                t.Title == LayoutFileManager.BootstrapDefaultTemplateName);
            if (bootstrapTemplate == null)
            {
                throw new ArgumentException("Bootstrap template not found");
            }

            return bootstrapTemplate;
        }

        private PageTemplate GetDefaultMinimalTemplate()
        {
            var pageManager = PageManager.GetManager();

            var minimalTemplate = pageManager.GetTemplates().FirstOrDefault(t => (t.Name == "Minimal.default" && t.Title == "default") ||
                t.Title == "Minimal.default");
            if (minimalTemplate == null)
            {
                throw new ArgumentException("Minimal template not found");
            }

            return minimalTemplate;
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
    }
}