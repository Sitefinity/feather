using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Web;
using MbUnit.Framework;
using Microsoft.VisualBasic.FileIO;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestIntegration.ResourcePackages
{
    [TestFixture]
    [Description("This is a class with tests related to feather resource packages.")]
    public class PackagesTests
    {
        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Adds new package with layout files and verifies the generated page templates.")]
        public void ResourcePackage_AddNewPackageWithLayoutFiles_VerifyGeneratedTemplates()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(Constants.PackageResource);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 3);

                string[] templateTitles = new string[] { Constants.TemplateTestLayout1, Constants.TemplateTestLayout2, Constants.TemplateTestLayout3 };

                foreach (var title in templateTitles)
                {
                    var template = this.PageManager.GetTemplates().Where(t => t.Title == title).FirstOrDefault();
                    Assert.IsNotNull(template, "Template was not found");                   
                }               
            }
            finally
            {
                string[] templates = new string[] { Constants.TemplateTestLayout1, Constants.TemplateTestLayout2, Constants.TemplateTestLayout3 };

                foreach (var template in templates)
                {                   
                    ServerOperations.Templates().DeletePageTemplate(template);                 
                }

                string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(Constants.TestPackageName);
                FeatherServerOperations.ResourcePackages().DeleteDirectory(path);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Adds new package without layout files and verifies there are no new generated page templates.")]
        public void ResourcePackage_AddNewPackageWithNoLayoutFiles_VerifyNotGeneratedTemplates()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(Constants.PackageNoLayoutsResource);
                Thread.Sleep(TimeSpan.FromSeconds(1));

                Assert.IsTrue(this.PageManager.GetTemplates().Count() == templatesCount);
            }
            finally
            {
                string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(Constants.PackageNoLayouts);
                FeatherServerOperations.ResourcePackages().DeleteDirectory(path);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Adds new resource package with layout files, rename the package and verifies the newly generated page templates.")]
        [Ignore("Failed integration test")]
        public void ResourcePackage_RenamePackageWithLayoutFiles_VerifyGeneratedTemplates()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(Constants.PackageResource2);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 3);

                string[] templateTitles = new string[] { Constants.TemplateTestLayout12, Constants.TemplateTestLayout22, Constants.TemplateTestLayout32 };

                foreach (var title in templateTitles)
                {
                    var template = this.PageManager.GetTemplates().Where(t => t.Title == title).FirstOrDefault();
                    Assert.IsNotNull(template, "Template was not found");
                }

                FeatherServerOperations.ResourcePackages().RenamePackageFolder(Constants.TestPackageName2, Constants.NewTestPackageName);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 6);

                string[] newTemplateTitles = new string[] { Constants.NewTemplateTestLayout1, Constants.NewTemplateTestLayout2, Constants.NewTemplateTestLayout3 };

                foreach (var title in newTemplateTitles)
                {
                    var template = this.PageManager.GetTemplates().Where(t => t.Title == title).FirstOrDefault();
                    Assert.IsNotNull(template, "Template was not found");
                }
            }
            finally
            {
                string[] templates = new string[] 
                { 
                    Constants.TemplateTestLayout1, 
                    Constants.TemplateTestLayout2, 
                    Constants.TemplateTestLayout3, 
                    Constants.NewTemplateTestLayout1, 
                    Constants.NewTemplateTestLayout2, 
                    Constants.NewTemplateTestLayout3 
                };

                foreach (var template in templates)
                {
                    ServerOperations.Templates().DeletePageTemplate(template);
                }

                string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(Constants.NewTestPackageName);
                FeatherServerOperations.ResourcePackages().DeleteDirectory(path);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Renames ResourcePackages folder and verifies that the templates are no loger based on the layout files.")]
        public void ResourcePackage_RenameMainPackagesFolder_VerifyTemplatesNotBasedOnLayouts()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            string template1Title = "Foundation.TestLayout";
            string template2Title = "Bootstrap.TestLayout";

            string page2 = "page2";

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath("Foundation", Constants.LayoutFileNameFoundation);
            string file2Path = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath("Bootstrap", Constants.LayoutFileNameBootstrap);

            string folderPath = Path.Combine(FeatherServerOperations.ResourcePackages().SfPath, "ResourcePackages");
            string folderName = "ResourcePackages";
            string newFolderName = "ResourcePackagesRenamed";
            string newFolderPath = Path.Combine(FeatherServerOperations.ResourcePackages().SfPath, "ResourcePackagesRenamed");

            try
            {
                // Add layout file to Foundation package               
                FeatherServerOperations.ResourcePackages().AddNewResource(Constants.LayoutFileResource, filePath);

                // Verify template is generated successfully
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);
                var template1 = this.PageManager.GetTemplates().Where(t => t.Title == template1Title).FirstOrDefault();
                Assert.IsNotNull(template1, "Template was not found");

                // Add layout file to Bootstrap package
                FeatherServerOperations.ResourcePackages().AddNewResource(Constants.LayoutFileResource, file2Path);

                // Verify template is generated successfully
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 2);
                var template2 = this.PageManager.GetTemplates().Where(t => t.Title == template2Title).FirstOrDefault();
                Assert.IsNotNull(template1, "Template was not found");

                // Create page with template from Foundation package and verify content
                Guid page1Id = FeatherServerOperations.Pages().CreatePageWithTemplate(template1, Constants.PageTitle, Constants.PageUrl);
                var content = FeatherServerOperations.Pages().GetPageContent(page1Id);
                Assert.IsTrue(content.Contains(Constants.LayoutTemplateText), "Layout text was not found on the page");

                // Create page with template from Bootstrap package and verify content
                Guid page2Id = FeatherServerOperations.Pages().CreatePageWithTemplate(template2, page2, page2);
                var content2 = FeatherServerOperations.Pages().GetPageContent(page2Id);
                Assert.IsTrue(content2.Contains(Constants.LayoutTemplateText), "Layout text was not found on the page");

                // Rename ResourcePackages folder
                if (Directory.Exists(folderPath))
                {
                    FeatherServerOperations.ResourcePackages().UnlockFolder(folderPath);
                    FileSystem.RenameDirectory(folderPath, newFolderName);
                }

                Assert.IsTrue(Directory.Exists(newFolderPath), "Renamed folder was not found");

                // Assert templates and pages are not based to layouts
                content = FeatherServerOperations.Pages().GetPageContent(page1Id);
                Assert.IsFalse(content.Contains(Constants.LayoutTemplateText), "Layout text was found on the page, but is shouldn't");

                content2 = FeatherServerOperations.Pages().GetPageContent(page2Id);
                Assert.IsFalse(content2.Contains(Constants.LayoutTemplateText), "Layout text was found on the page, but it shouldn't");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(template1Title);
                ServerOperations.Templates().DeletePageTemplate(template2Title);

                if (Directory.Exists(newFolderPath))
                {
                    FeatherServerOperations.ResourcePackages().UnlockFolder(newFolderPath);
                    FileSystem.RenameDirectory(newFolderPath, folderName);

                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        FileSystem.RenameDirectory(newFolderPath, folderName);
                    }

                    try
                    {
                        File.Delete(file2Path);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        FileSystem.RenameDirectory(newFolderPath, folderName);
                    }
                }
                else if (Directory.Exists(folderPath))
                {
                    File.Delete(filePath);
                    File.Delete(file2Path);
                }
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Adds new package with sample view for Mvc widget, creates new template based on the package and verifies the view is applied.")]
        public void ResourcePackage_CreateTemplateBasedOnlyOnThePackage_VerifyViewFromThePackage()
        {
            string pageName = "FeatherPage";
            string widgetCaption = "TestMvcWidget";
            string placeHolderId = "Contentplaceholder1";
            string packageName = "Bootstrap";
            string viewFileName = "Default.cshtml";
            string widgetName = "MvcTest";
            string fileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Default.cshtml";
            string templateTitle = "Bootstrap.default";
            string text = "This is a view from package.";

            try
            {
                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(packageName, widgetName, viewFileName);
                FeatherServerOperations.ResourcePackages().AddNewResource(fileResource, filePath);

                Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(templateTitle);

                Guid pageId = ServerOperations.Pages().CreatePage(pageName, templateId);
                pageId = ServerOperations.Pages().GetPageNodeId(pageId);

                FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId, typeof(MvcTestController).FullName, widgetCaption, placeHolderId);

                var content = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(content.Contains(text), "Template is not based on the package");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();

                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(packageName, widgetName, viewFileName);

                FileInfo fi = new FileInfo(filePath);
                DirectoryInfo di = fi.Directory;

                FeatherServerOperations.ResourcePackages().DeleteDirectory(di.FullName);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Adds new package without layout file and adds new widget view to the package, renames the template, keeping the package in the title and verifies the template content"),
        Ignore("Not stable")]
        public void ResourcePackage_RenameTemplateAndKeepThePackageInTheTitle_VerifyTemplateAndPage()
        {
            string pageName = "FeatherPage";
            string pageName2 = "FeatherPage2";
            string widgetCaption = "TestMvcWidget";
            string placeHolderId = "Body";
            string packageName = "Package1";
            string viewFileName = "Default.cshtml";
            string widgetName = "MvcTest";
            string fileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Default.cshtml";
            string packageResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Package1.zip";
            string templateTitle = "test-layout";
            string viewText = "This is a view from package.";
            string layoutText = "Package1 - test layout";
            string templateRenamed = "TemplateRenamed";

            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                // Add new package to the file system
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(packageResource);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                // Verify template is generated successfully
                var template = this.PageManager.GetTemplates().Where(t => t.Title == templateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found"); 

                // Add new view to the package
                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(packageName, widgetName, viewFileName);
                FeatherServerOperations.ResourcePackages().AddNewResource(fileResource, filePath);

                // Create page based on the new template from the package
                Guid pageId = ServerOperations.Pages().CreatePage(pageName, template.Id);
                this.PageManager.SaveChanges();
                pageId = ServerOperations.Pages().GetPageNodeId(pageId);

                // Verify the page content contains the text from the layout file
                var content = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(content.Contains(layoutText), "Template is not based on the layout file");

                // Rename the template
                template.Title = templateRenamed;
                template.Name = templateRenamed;
                this.PageManager.SaveChanges();

                // Create page based on the renamed template
                Guid pageId2 = ServerOperations.Pages().CreatePage(pageName2, template.Id);
                pageId2 = ServerOperations.Pages().GetPageNodeId(pageId2);

                FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId2, typeof(MvcTestController).FullName, widgetCaption, placeHolderId);

                // Verify the page content contains the text from the view added in the package
                var content2 = FeatherServerOperations.Pages().GetPageContent(pageId2);
                Assert.IsFalse(content2.Contains(layoutText), "Template is based on the layout file, but it shouldn't be");
                Assert.IsTrue(content2.Contains(viewText), "Template is not based on the package");
            }
            finally
            {
                string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(packageName);

                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(templateRenamed);
                ServerOperations.Templates().DeletePageTemplate(templateTitle);              
                FeatherServerOperations.ResourcePackages().DeleteDirectory(path);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Adds new package without layout file, creates a template and page, removes the package and verifies the page content")]
        [Ignore("The page returns a Server error after removing the package.")]
        public void ResourcePackage_DeleteResourcePackage_VerifyTemplateAndPageNotBasedOnLayout()
        {
            string pageName = "FeatherPage" + Guid.NewGuid().ToString();
            string packageName = "Package1";
            string packageResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Package1.zip";
            string templateTitle = "Package1.test-layout";
            string layoutText = "Package1 - test layout";
            string serverErrorMsg = "Server Error";

            PageManager pm = PageManager.GetManager();
            Guid pageId = ServerOperations.Pages().CreatePage(pageName);

            int templatesCount = pm.GetTemplates().Count();

            FeatherServerOperations.ResourcePackages().AddNewResourcePackage(packageResource);
            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

            var template = pm.GetTemplates().Where(t => t.Title == templateTitle).FirstOrDefault();
            Assert.IsNotNull(template, "Template was not found");

            string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(packageName);

            try
            {                
                ServerOperations.Templates().SetTemplateToPage(pageId, template.Id);

                var content = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsTrue(content.Contains(layoutText), "Template is not based on the layout file");

                FeatherServerOperations.ResourcePackages().DeleteDirectory(path);

                if (Directory.Exists(path))
                {
                    throw new ArgumentException("Directory was not deleted");
                }

                Thread.Sleep(1000);

                content = FeatherServerOperations.Pages().GetPageContent(pageId);
                Assert.IsFalse(content.Contains(serverErrorMsg), "Server Error was found on the page.");
                Assert.IsFalse(content.Contains(layoutText), "Template is still based on the layout file after package is deleted");
            }
            finally
            {
                ServerOperations.Pages().DeleteAllPages();
                ServerOperations.Templates().DeletePageTemplate(templateTitle);

                if (Directory.Exists(path))
                {
                    FeatherServerOperations.ResourcePackages().DeleteDirectory(path);
                }
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Adds new package without layout file, replaces it with the same package with different layouts and verifies new templates are generated"),
        Ignore("Unstable test")]
        public void ResourcePackage_ReplaceExistingPackage_VerifyNewTemplatesGenerated()
        {
            string packageName = "Package1";
            string packageResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Package1.zip";
            string templateTitle = "Package1.test-layout";
            string newPackageResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.ReplaceTest.Package1.zip";
            string newTemplateTitle = "Package1.replace-test";
            string tempFolder = "Temp";
            string packagesFolder = "ResourcePackages";
            string sitefinityPath = FeatherServerOperations.ResourcePackages().SfPath;

            PageManager pm = PageManager.GetManager();

            int templatesCount = pm.GetTemplates().Count();

            FeatherServerOperations.ResourcePackages().AddNewResourcePackage(packageResource);
            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

            var template = pm.GetTemplates().Where(t => t.Title == templateTitle).FirstOrDefault();
            Assert.IsNotNull(template, "Template was not found");

            var packagePath = Path.Combine(sitefinityPath, packagesFolder, packageName);

            string tempFolderPath = Path.Combine(sitefinityPath, tempFolder);

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(newPackageResource, tempFolder);

                var tempPath = Path.Combine(sitefinityPath, tempFolder, packageName);

                DirectoryInfo originalPackage = new DirectoryInfo(packagePath);
                DirectoryInfo newPackage = new DirectoryInfo(tempPath);

                MergeFolders(newPackage, originalPackage);

                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 2);
                Assert.IsTrue(pm.GetTemplates().Count().Equals(templatesCount + 2), "templates count is not correct");

                var newTemplate = pm.GetTemplates().Where(t => t.Title == newTemplateTitle).FirstOrDefault();
                Assert.IsNotNull(newTemplate, "New template was not found");
            }
            finally
            {
                ServerOperations.Templates().DeletePageTemplate(templateTitle);
                ServerOperations.Templates().DeletePageTemplate(newTemplateTitle);
                FeatherServerOperations.ResourcePackages().DeleteDirectory(tempFolderPath);
                FeatherServerOperations.ResourcePackages().DeleteDirectory(packagePath);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        private static void MergeFolders(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo dirSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(dirSourceSubDir.Name);
                MergeFolders(dirSourceSubDir, nextTargetSubDir);
            }
        }

        private PageManager pageManager;

        private PageManager PageManager
        {
            get
            {
                if (this.pageManager == null)
                {
                    this.pageManager = PageManager.GetManager();
                }

                return this.pageManager;
            }
        }
    }
}
