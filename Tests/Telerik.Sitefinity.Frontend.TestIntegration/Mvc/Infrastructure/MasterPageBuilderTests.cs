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
        /// Checks whether default form tag exist for bootstrap package in preview mode
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks for default aspnetForm tag for Bootstrap package in preview mode.")]
        public void CreatePage_Bootstrap_CheckFormTagNotExistPreviewMode()
        {
            Guid bootstrapPageId = Guid.Empty;
            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();

            try
            {
                var suffix = Guid.NewGuid().ToString("N");
                var bootstrapTemplate = this.GetDefaultBootstrapTemplate();
                var bootstrapPageName = "PageBootstrap";
                var previewFormTagPattern = "<form[\\s].*[\\s]?id=\"aspnetForm\".*>.*";

                // page preview bootstrap
                bootstrapPageId = FeatherServerOperations.Pages().CreatePageWithTemplate(bootstrapTemplate, bootstrapPageName + suffix, bootstrapPageName + suffix);
                var bootstrapPagePreviewFormTagsCount = this.PagePreviewFormTags(bootstrapPageId, previewFormTagPattern);

                // template preview bootstrap
                var bootstrapTemplatePreviewFormTagsCount = this.TemplatePreviewFormTags(bootstrapTemplate, previewFormTagPattern);

                Assert.IsFalse(bootstrapPagePreviewFormTagsCount == 1, "Default ASP.Net form tag exist in Bootstrap package in page preview mode");
                Assert.IsFalse(bootstrapTemplatePreviewFormTagsCount == 1, "Default ASP.Net form tag exist for Bootstrap package in template preview mode");
            }
            finally
            {
                if (bootstrapPageId != Guid.Empty)
                {
                    ServerOperations.Pages().DeletePage(bootstrapPageId);
                }
            }
        }

        /// <summary>
        /// Ignored - Minimal package is not enabled on Jenkins setup
        /// </summary>
        [Ignore]
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks for default aspnetForm tag for Minimal package in preview mode.")]
        public void CreatePage_Minimal_CheckFormTagNotExistPreviewMode()
        {
            Guid minimalPageId = Guid.Empty;
            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();

            try
            {
                var suffix = Guid.NewGuid().ToString("N");
                var minimalTemplate = this.GetDefaultMinimalTemplate();
                var minimalPageName = "PageMinimal";
                var previewFormTagPattern = "<form[\\s].*[\\s]?id=\"aspnetForm\".*>.*";

                // page preview minimal
                minimalPageId = FeatherServerOperations.Pages().CreatePageWithTemplate(minimalTemplate, minimalPageName + suffix, minimalPageName + suffix);
                var minimalPagePreviewFormTagsCount = this.PagePreviewFormTags(minimalPageId, previewFormTagPattern);

                // template preview minimal
                var minimalTemplatePreviewFormTagsCount = this.TemplatePreviewFormTags(minimalTemplate, previewFormTagPattern);

                Assert.IsFalse(minimalPagePreviewFormTagsCount == 1, "Default ASP.Net form tag exist for Minimal package in page preview mode");
                Assert.IsFalse(minimalTemplatePreviewFormTagsCount == 1, "Default ASP.Net form tag exist for Minimal package in template preview mode");
            }
            finally
            {
                if (minimalPageId != Guid.Empty)
                {
                    ServerOperations.Pages().DeletePage(minimalPageId);
                }
            }
        }

        private int TemplatePreviewFormTags(PageTemplate template, string previewFormTagPattern)
        {
            var templatePreviewPageUrl = string.Concat(
                HttpContext.Current.Request.Url.Scheme,
                "://",
                HttpContext.Current.Request.Url.Host,
                "/Sitefinity/Template/",
                template.Id,
                "/Preview/");
            string templatePreviewContent = this.GetContent(templatePreviewPageUrl);
            var templatePreviewFormTagsCount = Regex.Matches(templatePreviewContent, previewFormTagPattern, RegexOptions.IgnoreCase).Count;

            return templatePreviewFormTagsCount;
        }

        private int PagePreviewFormTags(Guid pageId, string previewFormTagPattern)
        {
            string pagePreviewContent = FeatherServerOperations.Pages().GetPageContent(pageId, true, "action/preview");
            var pagePreviewFormTagsCount = Regex.Matches(pagePreviewContent, previewFormTagPattern, RegexOptions.IgnoreCase).Count;

            return pagePreviewFormTagsCount;
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