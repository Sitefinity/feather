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
            Guid pageId = Guid.Empty;
            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();

            try
            {
                var suffix = Guid.NewGuid().ToString("N");
                var bootstrapTemplate = GetDefaultBootstrapTemplate();
                var placeholder = "Contentplaceholder1";
                var pageName = "FormsPageBootstrap";
                var previewFormTagPattern = "<form[\\s].*[\\s]?id=\"aspnetForm\".*>.*";

                // page preview
                pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(bootstrapTemplate, pageName + suffix, pageName + suffix);

                FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId, "RegistrationController", "Registration Controller Widget", placeholder);
                FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId, "ProfileController", "Profile Controller Widget", placeholder);

                string pagePreviewContent = FeatherServerOperations.Pages().GetPageContent(pageId, true, "action/preview");
                var pagePreviewFormTagsCount = Regex.Matches(pagePreviewContent, previewFormTagPattern, RegexOptions.IgnoreCase).Count;

                // template preview
                var templatePreviewPageUrl = string.Concat(
                    HttpContext.Current.Request.Url.Scheme,
                    "://",
                    HttpContext.Current.Request.Url.Host,
                    "/Sitefinity/Template/",
                    bootstrapTemplate.Id,
                    "/Preview/");
                string templatePreviewContent = this.GetContent(templatePreviewPageUrl);
                var templatePreviewFormTagsCount = Regex.Matches(templatePreviewContent, previewFormTagPattern, RegexOptions.IgnoreCase).Count;

                Assert.IsFalse(pagePreviewFormTagsCount == 1, "Default ASP.Net form tag exist in page preview mode");
                Assert.IsFalse(templatePreviewFormTagsCount == 1, "Default ASP.Net form tag exist in template preview mode");
            }
            finally
            {
                if (pageId != Guid.Empty)
                {
                    ServerOperations.Pages().DeletePage(pageId);
                }
            }
        }

        private static PageTemplate GetDefaultBootstrapTemplate()
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