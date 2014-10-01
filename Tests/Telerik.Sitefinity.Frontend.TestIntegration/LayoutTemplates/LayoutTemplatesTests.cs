using System;
using System.IO;
using System.Linq;
using System.Threading;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.LayoutTemplates
{
    [TestFixture]
    [Description("This is a class with tests related to layout templates in mvc file structure.")]
    public class LayoutTemplatesTests
    {
        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Adds new layout file to Mvc/Views/Layouts, verifies the automatically generated page template and creates a page based on the template in order to verify the content.")]
        public void LayoutTemplates_AddNewLayoutFile_VerifyGeneratedTemplateAndCreatedPageContent()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            var folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            try
            {               
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, LayoutFileName);

                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);

                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                var page = pageManager.GetPageNode(pageId);
                var pageUrl = page.GetFullUrl();
                pageUrl = RouteHelper.GetAbsoluteUrl(pageUrl);

                string pageContent = WebRequestHelper.GetPageWebContent(pageUrl);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
              
                var filePath = Path.Combine(folderPath, LayoutFileName);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Adds new layout file to Mvc/Views/Layouts, edits the file and verifies the automatically generated page template and a page based on the template.")]
        public void LayoutTemplates_EditLayoutFile_VerifyGeneratedTemplateAndCreatedPageContent()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            var folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, LayoutFileName);

                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);

                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                var page = pageManager.GetPageNode(pageId);
                var pageUrl = page.GetFullUrl();
                pageUrl = RouteHelper.GetAbsoluteUrl(pageUrl);

                string pageContent = WebRequestHelper.GetPageWebContent(pageUrl);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");

                string layoutFile = Path.Combine(folderPath, LayoutFileName);
                FeatherServerOperations.ResourcePackages().EditLayoutFile(layoutFile, TestLayoutTemplateText, TestLayoutTemplateTextEdited);

                this.PublishPage(page);
                pageContent = WebRequestHelper.GetPageWebContent(pageUrl);

                Assert.IsFalse(pageContent.Contains(ServerErrorMessage), "Page throws a server error message");
                Assert.IsFalse(pageContent.Contains(TestLayoutTemplateText), "Layout template text was found in the page content");
                Assert.IsTrue(pageContent.Contains(TestLayoutTemplateTextEdited), "New layout text was not found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

                var filePath = Path.Combine(folderPath, LayoutFileName);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Adds new layout file to Mvc/Views/Layouts calculating the current DateTime, verify the page content is not updated on the frontend.")]
        [Ignore("Ignoring this test because of an issue, related to layout templates. Remove the attribute when the issue is fixed.")]
        public void LayoutTemplates_AddLayoutFileRenderingDateTime_VerifyTemplateNotPrecompiled()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            var folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, DateTimeLayoutFile);

                FeatherServerOperations.ResourcePackages().AddNewResource(DateTimeLayoutResource, filePath);

                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == DateTimeTemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                string pageContent = this.GetPageContent(pageId);
             
                Thread.Sleep(1000);

                string newContent = this.GetPageContent(pageId);
                ////Test this before and after the bug is fixed, because it always passes
                Assert.AreEqual(pageContent, newContent, "Page content is updated, but it shouldn't");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(DateTimeTemplateTitle);

                var filePath = Path.Combine(folderPath, DateTimeLayoutFile);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Creates a page template and adds new layout file to Mvc/Views/Layouts, verifies the template is based on the layout.")]
        public void LayoutTemplates_CreateTemplateAndAddLayoutFile_VerifyTemplateBasedOnLayout()
        {
            var folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            try
            {
                var templateId = ServerOperations.Templates().CreatePureMVCPageTemplate(TemplateTitle);
                Guid pageId = ServerOperations.Pages().CreatePage(PageTitle, templateId);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, LayoutFileName);

                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);

                var nodeId = ServerOperations.Pages().GetPageNodeId(pageId);
                var pageContent = this.GetPageContent(nodeId);
                Assert.IsTrue(pageContent.Contains(TestLayoutTemplateText), "Layout text was not found on the page");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

                var filePath = Path.Combine(folderPath, LayoutFileName);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Creates a page template and adds layout files to Mvc/Views/Layouts with special characters in the names, verifies the template is based on the correct layout.")]
        public void LayoutTemplates_CreateTemplateAndAddLayoutFilesUsingSpecialCharacters_VerifyTemplateBasedOnCorrectLayout()
        {
            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            string layout1Resource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.LayoutFilesSpecialChars.My@TestTemplate.cshtml";
            string layout2Resource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.LayoutFilesSpecialChars.My_TestTemplate.cshtml";
            string layout1Name = "My@TestTemplate.cshtml";
            string layout2Name = "My_TestTemplate.cshtml";
            string templateTitle = "My@TestTemplate";
            string layout1Text = "My@TestTemplate";
            string layout2Text = "My_TestTemplate";

            try
            {
                var templateId = ServerOperations.Templates().CreatePureMVCPageTemplate(templateTitle);
                Guid pageId = ServerOperations.Pages().CreatePage(PageTitle, templateId);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string file1Path = Path.Combine(folderPath, layout1Name);
                FeatherServerOperations.ResourcePackages().AddNewResource(layout1Resource, file1Path);

                string file2Path = Path.Combine(folderPath, layout2Name);
                FeatherServerOperations.ResourcePackages().AddNewResource(layout2Resource, file2Path);

                var nodeId = ServerOperations.Pages().GetPageNodeId(pageId);
                var pageContent = this.GetPageContent(nodeId);
                Assert.IsTrue(pageContent.Contains(layout1Text), "Layout1 text was not found");
                Assert.IsFalse(pageContent.Contains(layout2Text), "Layout2 text is found, but it shouldn't be");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

                string file1Path = Path.Combine(folderPath, layout1Name);
                File.Delete(file1Path);

                string file2Path = Path.Combine(folderPath, layout2Name);
                File.Delete(file2Path);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Renames layout file from Mvc/Views/Layouts, verifies new template is generated.")]
        public void LayoutTemplates_RenameLayoutFile_VerifyNewGeneratedTemplate()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string layoutFileNewName = "TestLayoutRenamed.cshtml";
            string newTemplateTitle = "TestLayoutRenamed";

            string filePath = Path.Combine(folderPath, LayoutFileName);
            string newFilePath = Path.Combine(folderPath, layoutFileNewName);

            try
            {               
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");
            
                File.Move(filePath, newFilePath);

                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 2);

                var newTemplate = pageManager.GetTemplates().Where(t => t.Title == newTemplateTitle).FirstOrDefault();
                Assert.IsNotNull(newTemplate, "Template was not found");
            }
            finally
            {
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
                ServerOperations.Templates().DeletePageTemplate(newTemplateTitle);
                File.Delete(filePath);
                File.Delete(newFilePath);
            }
        }

        private string SfPath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/");
            }
        }

        private void PublishPage(PageNode page)
        {
            PageManager pageManager = PageManager.GetManager();
            var pageData = page.GetPageData();
            var master = pageManager.PagesLifecycle.GetMaster(pageData);
            pageManager.PagesLifecycle.Publish(master);
            pageManager.SaveChanges();
        }

        private string GetPageContent(Guid pageId)
        {
            PageManager pageManager = PageManager.GetManager();

            var page = pageManager.GetPageNode(pageId);
            var pageUrl = page.GetFullUrl();
            pageUrl = RouteHelper.GetAbsoluteUrl(pageUrl);

            string pageContent = WebRequestHelper.GetPageWebContent(pageUrl);

            return pageContent;
        }

        private const string LayoutFileName = "TestLayout.cshtml";
        private const string DateTimeLayoutFile = "DateTimeLayout.cshtml";
        private const string TemplateTitle = "TestLayout";
        private const string DateTimeTemplateTitle = "DateTimeLayout";
        private const string LayoutFileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        private const string DateTimeLayoutResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.DateTimeLayout.cshtml";
        private const string PageTitle = "FeatherPage";
        private const string PageUrl = "featherpage";
        private const string LayoutTemplateText = "Test Layout";
        private const string ServerErrorMessage = "Server Error";
        private const string TestLayoutTemplateText = "Test Layout";
        private const string TestLayoutTemplateTextEdited = "New Text";
    }
}
