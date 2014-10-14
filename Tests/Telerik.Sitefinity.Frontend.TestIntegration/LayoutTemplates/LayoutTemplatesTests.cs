using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.TestUtilities;
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

                Thread.Sleep(1000);

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
        public void LayoutTemplates_AddLayoutFileRenderingDateTime_VerifyTemplateNotRecompiled()
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

                FeatherServerOperations.Pages().GetPageContent(pageId);
                var recompCount = SystemMonitoring.GetRecompilationCount();

                Thread.Sleep(1000);

                FeatherServerOperations.Pages().GetPageContent(pageId);

                Assert.AreEqual(recompCount, SystemMonitoring.GetRecompilationCount(), "Unexpected recompilation happened.");
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
                var pageContent = FeatherServerOperations.Pages().GetPageContent(nodeId);
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
            string template1Title = "My@TestTemplate";
            string template2Title = "My_TestTemplate";
            string layout1Text = "My@TestTemplate";
            string layout2Text = "My_TestTemplate";

            try
            {
                var templateId = ServerOperations.Templates().CreatePureMVCPageTemplate(template1Title);
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
                var pageContent = FeatherServerOperations.Pages().GetPageContent(nodeId);
                Assert.IsTrue(pageContent.Contains(layout1Text), "Layout1 text was not found");
                Assert.IsFalse(pageContent.Contains(layout2Text), "Layout2 text is found, but it shouldn't be");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(template1Title);
                ServerOperations.Templates().DeletePageTemplate(template2Title);

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

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Replace layout file in Mvc/Views/Layouts, verifies the template and the page that uses it.")]
        public void LayoutTemplates_ReplaceLayoutFile_VerifyGeneratedTemplateAndCreatedPageContent()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string tempFolderPath = Path.Combine(this.SfPath, "MVC", "Views", "Temp");

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }

            string newLayoutTemplateText = "Test Layout Replaced";
            string layoutResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.LayoutFileReplace.TestLayout.cshtml";
            string backFileName = "TestLayout.cshtml.bac";
            string filePath = Path.Combine(folderPath, LayoutFileName);
            string tempFilePath = Path.Combine(tempFolderPath, LayoutFileName);           
            string backFilePath = Path.Combine(folderPath, backFileName);

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                string pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");

                FeatherServerOperations.ResourcePackages().AddNewResource(layoutResource, tempFilePath);
                File.Replace(tempFilePath, filePath, backFilePath);

                Thread.Sleep(1000);

                pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);

                Assert.AreEqual(pageManager.GetTemplates().Count(), templatesCount + 1, "Unnecessary template was generated");
                Assert.IsTrue(pageContent.Contains(newLayoutTemplateText), "New layout text was not found in the page content after replace");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
                File.Delete(filePath);
                File.Delete(backFilePath);
                FeatherServerOperations.ResourcePackages().DeleteDirectory(tempFolderPath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Rename template based on layout file in Mvc/Views/Layouts, verifies the template and the page that uses it.")]
        public void LayoutTemplates_RenameTemplateBasedOnLayoutFile_VerifyTemplateAndPage()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, LayoutFileName);
            string templateRenamed = "TemplateRenamed";
            string serverErrorMessage = "Server Error";

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                string pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");

                template.Title = templateRenamed;
                template.Name = templateRenamed;
                pageManager.SaveChanges();

                Thread.Sleep(1000);

                pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);

                Assert.IsFalse(pageContent.Contains(serverErrorMessage), "Page throws a server error message");
                Assert.IsFalse(pageContent.Contains(LayoutTemplateText), "Layout template text was found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(templateRenamed);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Deletes layout file from Mvc/Views/Layouts, verifies the template and the page no longer use it.")]
        [Ignore("It will work if the page is republished but this is would not be a valid test. We need cache dependency in the page resolver to its master page.")]
        public void LayoutTemplates_DeleteLayoutFile_VerifyTemplateAndPageNotBasedToLayout()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, LayoutFileName);
            string serverErrorMessage = "Server Error";

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                string pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");

                File.Delete(filePath);

                Thread.Sleep(1000);

                pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);

                Assert.IsFalse(pageContent.Contains(serverErrorMessage), "Page throws a server error message");
                Assert.IsFalse(pageContent.Contains(LayoutTemplateText), "Layout template text was found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Creates template based on parent template generated from layout file from Mvc/Views/Layouts, verifies the page content that uses it.")]
        public void LayoutTemplates_CreateTemplateBasedOnGeneratedTemplate_VerifyPageContent()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, LayoutFileName);
            string newTemplateTitle = "NewTemplate";

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                var newTemplate = FeatherServerOperations.Pages().CreatePureMvcTemplate(newTemplateTitle, TemplateTitle);

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(newTemplate, PageTitle, PageUrl);

                string pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(newTemplateTitle);
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Creates template based on parent template generated from layout file from Mvc/Views/Layouts, edit the layout file and verifies the page content that uses the child template.")]
        public void LayoutTemplates_EditLayoutFile_VerifyTemplateBasedOnParentTemplateAndPageContent()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, LayoutFileName);
            string newTemplateTitle = "NewTemplate";
            string newContent = "New Layout File Content";
            string serverErrorMessage = "Server Error";

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                var newTemplate = FeatherServerOperations.Pages().CreatePureMvcTemplate(newTemplateTitle, TemplateTitle);              

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(newTemplate, PageTitle, PageUrl);

                string pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");

                FeatherServerOperations.ResourcePackages().EditLayoutFile(filePath, LayoutTemplateText, newContent);

                Thread.Sleep(1000);

                pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);

                Assert.IsFalse(pageContent.Contains(serverErrorMessage), "Page throws a server error message");
                Assert.IsFalse(pageContent.Contains(LayoutTemplateText), "Layout template text was found in the page content");
                Assert.IsTrue(pageContent.Contains(newContent), "New layout text was not found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(newTemplateTitle);
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Creates template with space in the name and verifies it is not based on layout file from Mvc/Views/Layouts")]
        public void LayoutTemplates_CreateTemplateUsingTitleWithSpace_VerifyTemplateNotBasedOnLayoutFile()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, LayoutFileName);
            string templateTitle = "   TestLayout";
            string page2 = "page2";

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                string pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");

                Guid templateId = FeatherServerOperations.Pages().CreatePureMvcTemplate(templateTitle);

                var template2 = pageManager.GetTemplates().Where(t => t.Id == templateId).FirstOrDefault();
                Assert.IsNotNull(template2, "Template was not found");

                Guid page2Id = FeatherServerOperations.Pages().CreatePageWithTemplate(template2, page2, page2);

                string page2Content = FeatherServerOperations.Pages().GetPageContent(page2Id);
                Assert.IsFalse(page2Content.Contains(LayoutTemplateText), "Layout template text was found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(templateTitle);
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.LayoutTemplates)]
        [Author("Petya Rachina")]
        [Description("Adds layout file to Mvc/Views/Layouts folder, creates new template based on convention and verifies the page that uses it")]
        public void LayoutTemplates_AddLayoutFileAndCreateNewTemplate_VerifyPageContent()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string folderPath = Path.Combine(this.SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, LayoutFileName);

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                ////Deleting the template in order to create new one with the same name
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

                template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNull(template, "Template was not found");

                ////Creating new template with the same title
                Guid templateId = FeatherServerOperations.Pages().CreatePureMvcTemplate(TemplateTitle);

                template = pageManager.GetTemplates().Where(t => t.Id == templateId).FirstOrDefault();
                Assert.IsNotNull(template, "New template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageTitle, PageUrl);

                string pageContent = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(pageContent.Contains(LayoutTemplateText), "Layout template text was not found in the page content");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
                File.Delete(filePath);
            }
        }

        private string SfPath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/");
            }
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
