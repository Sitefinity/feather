using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.ActionFilters;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Helpers
{
    /// <summary>
    /// This class contains tests methods for the <see cref="ResourceHelper" />
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This is a class with tests related to resource helpers.")]
    public class ResourceHelperTests
    {
        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ActionExecutionRegister.ExecutedActionInfos.Clear();

            ServerOperations.Pages().DeleteAllPages();
        }

        #region Public Methods and Operators

        /// <summary>
        /// Ensures that a JavaScript is registered only in ScriptManager for hybrid pages.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that a JavaScript is registered only in ScriptManager for hybrid pages.")]
        public void RegisterScript_HybridPage_AddedInScriptManagerOnly()
        {
            string testName = "RegisterScript";
            string pageTitle = testName + "WebFormsPage";
            string urlName = testName + "WebFormsPage";
            string pageUrl = UrlPath.ResolveAbsoluteUrl("~/" + urlName);

            this.AddDummyScriptControllerToPage(pageTitle, urlName, null);

            WebRequestHelper.GetPageWebContent(pageUrl);

            Assert.AreEqual(2, ActionExecutionRegister.ExecutedActionInfos.Count, "The actions are not executed correctly.");

            for (int i = 0; i < 2; i++)
            {
                var result = ActionExecutionRegister.ExecutedActionInfos[i].Result as ContentResult;
                Assert.IsTrue(string.IsNullOrEmpty(result.Content), "The script should not be added outside of ScriptManager.");

                var httpContext = ActionExecutionRegister.ExecutedActionInfos[i].CurrentHttpContext;
                var scriptManagerScripts = System.Web.UI.ScriptManager.GetCurrent(httpContext.Handler.GetPageHandler()).Scripts;
                Assert.AreEqual(3, scriptManagerScripts.Count(), "The script is not added correctly");
            }          
        }

        /// <summary>
        /// Ensures that a JavaScript is registered only in ScriptManager for pure MVC pages.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that a JavaScript is registered only in ScriptManager for pure MVC pages.")]
        public void RegisterScript_PureMvcPage_AddedOnce()
        {
            string testName = "RegisterScript";
            string pageTitle = testName + "PureMvcPage";
            string urlName = testName + "PureMvcPage";
            string pageUrl = UrlPath.ResolveAbsoluteUrl("~/" + urlName);

            var pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            try
            {
                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(ResourcePackages.Constants.PackageName, ResourcePackages.Constants.LayoutFileName);
                FeatherServerOperations.ResourcePackages().AddNewResource(ResourcePackages.Constants.LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == ResourcePackages.Constants.TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                this.AddDummyScriptControllerToPage(pageTitle, urlName, template, "TestPlaceHolder");

                WebRequestHelper.GetPageWebContent(pageUrl);

                Assert.AreEqual(2, ActionExecutionRegister.ExecutedActionInfos.Count, "The actions are not executed correctly.");

                var result1 = ActionExecutionRegister.ExecutedActionInfos[0].Result as ContentResult;
                Assert.IsTrue(Regex.IsMatch(result1.Content, "<script src=\".*\" type=\"text/javascript\"></script>"), "The script is not added.");

                var result2 = ActionExecutionRegister.ExecutedActionInfos[1].Result as ContentResult;
                Assert.IsTrue(string.IsNullOrEmpty(result2.Content), "The script should not be added twice.");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(ResourcePackages.Constants.TemplateTitle);

                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(ResourcePackages.Constants.PackageName, ResourcePackages.Constants.LayoutFileName);
                File.Delete(filePath);
            }
        }

        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that when two widgets on a page based on a layout register the same script in two sections only the reference in the top section is rendered.")]
        public void PageBasedOnLayoutTwoWidgets_RegisterBottomAndTopSameScript_TopReferenceRendered()
        {
            var scriptSource = "http://test.cdn.com/test-script.js";
            string testName = System.Reflection.MethodInfo.GetCurrentMethod().Name;

            using (var setup = new PageOnTestLayoutSetup(testName))
            {
                this.AddScriptControllerToPage(setup.PageId, scriptSource, "bottom");
                this.AddScriptControllerToPage(setup.PageId, scriptSource, "top");

                string pageContent = setup.GetPageContent();
                var encodedScriptSource = HttpUtility.HtmlEncode(scriptSource);
                int count = new Regex(Regex.Escape(encodedScriptSource), RegexOptions.IgnoreCase).Matches(pageContent).Count;

                Assert.AreEqual(1, count, "The script reference is rendered more than once or none at all.");
                Assert.IsTrue(this.IsInSection("top", encodedScriptSource, pageContent), "The script reference was not in the expected section.");
            }
        }

        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that when two widgets on a page based on a layout register the same script, one inline and one on in top section, the one in the section is rendered.")]
        public void PageBasedOnLayoutTwoWidgets_RegisterInlineAndTopSameScript_TopReferenceRendered()
        {
            var scriptSource = "http://test.cdn.com/test-script.js";
            string testName = System.Reflection.MethodInfo.GetCurrentMethod().Name;

            using (var setup = new PageOnTestLayoutSetup(testName))
            {
                this.AddScriptControllerToPage(setup.PageId, scriptSource, null);
                this.AddScriptControllerToPage(setup.PageId, scriptSource, "top");

                string pageContent = setup.GetPageContent();
                var encodedScriptSource = HttpUtility.HtmlEncode(scriptSource);

                Assert.IsTrue(new Regex(Regex.Escape(encodedScriptSource), RegexOptions.IgnoreCase).IsMatch(pageContent), "The script reference was not rendered.");
                Assert.IsTrue(this.IsInSection("top", encodedScriptSource, pageContent), "The script reference was not in the expected section.");
            }
        }

        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that when two widgets on a page based on a layout register the same script, both inline, only one is rendered.")]
        public void PageBasedOnLayoutTwoWidgets_RegisteredScriptInline_RenderedOnce()
        {
            var scriptSource = "http://test.cdn.com/test-script.js";
            string testName = System.Reflection.MethodInfo.GetCurrentMethod().Name;

            using (var setup = new PageOnTestLayoutSetup(testName))
            {
                this.AddScriptControllerToPage(setup.PageId, scriptSource, null);
                this.AddScriptControllerToPage(setup.PageId, scriptSource, null);

                string pageContent = setup.GetPageContent();
                var encodedScriptSource = HttpUtility.HtmlEncode(scriptSource);

                int count = new Regex(Regex.Escape(encodedScriptSource), RegexOptions.IgnoreCase).Matches(pageContent).Count;
                Assert.AreEqual(1, count, "The script reference is rendered more than once or none at all.");
            }
        }

        #endregion

        #region Helper methods

        private void AddDummyScriptControllerToPage(string pageTitlePrefix, string urlNamePrefix, PageTemplate template, string placeHolder = "Body")
        {
            var controls = new List<System.Web.UI.Control>();
            var mvcProxy1 = new MvcControllerProxy();
            mvcProxy1.ControllerName = typeof(DummyScriptController).FullName;
            var controller1 = new DummyScriptController();
            mvcProxy1.Settings = new ControllerSettings(controller1);
            controls.Add(mvcProxy1);

            var mvcProxy2 = new MvcControllerProxy();
            mvcProxy2.ControllerName = typeof(DummyScriptController).FullName;
            var controller2 = new DummyScriptController();
            mvcProxy2.Settings = new ControllerSettings(controller2);
            controls.Add(mvcProxy2);

            Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(
                template,
                pageTitlePrefix,
                urlNamePrefix);

            PageContentGenerator.AddControlsToPage(pageId, controls, placeHolder);
        }

        private void AddScriptControllerToPage(Guid pageId, string scriptSource = null, string sectionName = null)
        {
            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = typeof(DummyScriptController).FullName;
            var controller = new DummyScriptController();
            controller.ScriptSource = scriptSource;
            controller.SectionName = sectionName;
            mvcProxy.Settings = new ControllerSettings(controller);

            PageContentGenerator.AddControlToPage(pageId, mvcProxy, Guid.NewGuid().ToString("N"), "TestPlaceHolder");
        }

        private bool IsInSection(string sectionName, string subString, string pageContent)
        {
            var regExpression = Regex.Escape("<div class=\"{0}-section\">".Arrange(sectionName)) + "(?!</div>).*" + Regex.Escape(subString) + ".*" + Regex.Escape("</div>");
            return new Regex(regExpression, RegexOptions.IgnoreCase).IsMatch(pageContent);
        }

        private class PageOnTestLayoutSetup : IDisposable
        {
            public Guid PageId
            {
                get
                {
                    return this.pageId;
                }
            }

            public PageOnTestLayoutSetup(string testName)
            {
                PageManager pageManager = PageManager.GetManager();
                int templatesCount = pageManager.GetTemplates().Count();
                this.folderPath = Path.Combine(HostingEnvironment.MapPath("~/"), "MVC", "Views", "Layouts");
                if (!Directory.Exists(this.folderPath))
                {
                    Directory.CreateDirectory(this.folderPath);
                }

                string filePath = Path.Combine(this.folderPath, PageOnTestLayoutSetup.LayoutFileName);
                FeatherServerOperations.ResourcePackages().AddNewResource(PageOnTestLayoutSetup.LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == PageOnTestLayoutSetup.LayoutName).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                this.pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, testName, testName);
            }

            public string GetPageContent()
            {
                var page = PageManager.GetManager().GetPageNode(this.pageId);
                var pageUrl = page.GetFullUrl();
                pageUrl = RouteHelper.GetAbsoluteUrl(pageUrl);

                return WebRequestHelper.GetPageWebContent(pageUrl);
            }

            public void Dispose()
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(PageOnTestLayoutSetup.LayoutName);

                var filePath = Path.Combine(this.folderPath, PageOnTestLayoutSetup.LayoutFileName);
                File.Delete(filePath);
            }

            private readonly Guid pageId;
            private readonly string folderPath;

            private const string LayoutName = "TestLayout";
            private const string LayoutFileName = "TestLayout.cshtml";
            private const string LayoutFileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        }

        #endregion
    }
}