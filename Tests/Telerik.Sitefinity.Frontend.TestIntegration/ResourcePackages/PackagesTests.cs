using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Web;
using MbUnit.Framework;
using Microsoft.VisualBasic.FileIO;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
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
        [Author("Petya Rachina")]
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
        [Author("Petya Rachina")]
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
        [Author("Petya Rachina")]
        [Description("Adds new resource package with layout files, rename the package and verifies the newly generated page templates.")]
        public void ResourcePackage_RenamePackageWithLayoutFiles_VerifyGeneratedTemplates()
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

                FeatherServerOperations.ResourcePackages().RenamePackageFolder(Constants.TestPackageName, Constants.NewTestPackageName);
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
        [Author("Petya Rachina")]
        [Description("Renames ResourcePackages folder and verifies that the templates are no loger based on the layout files.")]
        public void ResourcePackage_RenameMainPackagesFolder_VerifyTemplatesNotBasedOnLayouts()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            string template1Title = "Foundation.TestLayout";
            string template2Title = "Bootstrap.TestLayout";

            string page2 = "page2";

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath("Foundation", Constants.LayoutFileName);
            string file2Path = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath("Bootstrap", Constants.LayoutFileName);

            string folderPath = Path.Combine(FeatherServerOperations.ResourcePackages().SfPath, "ResourcePackages");
            string folderName = "ResourcePackages";
            string newFolderName = "ResourcePackagesRenamed";
            string newFolderPath = Path.Combine(FeatherServerOperations.ResourcePackages().SfPath, "ResourcePackagesRenamed");

            try
            {
                ////Add layout file to Foundation package               
                FeatherServerOperations.ResourcePackages().AddNewResource(Constants.LayoutFileResource, filePath);

                ////Verify template is generated successfully
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);
                var template1 = this.PageManager.GetTemplates().Where(t => t.Title == template1Title).FirstOrDefault();
                Assert.IsNotNull(template1, "Template was not found");

                ////Add layout file to Bootstrap package
                FeatherServerOperations.ResourcePackages().AddNewResource(Constants.LayoutFileResource, file2Path);

                ////Verify template is generated successfully
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 2);
                var template2 = this.PageManager.GetTemplates().Where(t => t.Title == template2Title).FirstOrDefault();
                Assert.IsNotNull(template1, "Template was not found");

                ////Create page with template from Foundation package and verify content
                Guid page1Id = FeatherServerOperations.Pages().CreatePageWithTemplate(template1, Constants.PageTitle, Constants.PageUrl);
                var content = FeatherServerOperations.Pages().GetPageContent(page1Id);
                Assert.IsTrue(content.Contains(Constants.LayoutTemplateText), "Layout text was not found on the page");

                ////Create page with template from Bootstrap package and verify content
                Guid page2Id = FeatherServerOperations.Pages().CreatePageWithTemplate(template2, page2, page2);
                var content2 = FeatherServerOperations.Pages().GetPageContent(page2Id);
                Assert.IsTrue(content2.Contains(Constants.LayoutTemplateText), "Layout text was not found on the page");

                ////Rename ResourcePackages folder
                if (Directory.Exists(folderPath))
                {
                    this.UnlockFolder(folderPath);
                    FileSystem.RenameDirectory(folderPath, newFolderName);
                }

                Assert.IsTrue(Directory.Exists(newFolderPath), "Renamed folder was not found");

                ////Assert templates and pages are not based to layouts
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
                    this.UnlockFolder(newFolderPath);
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

        private void UnlockFolder(string folderPath)
        {
            var account = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null).Translate(typeof(NTAccount)).Value;
            DirectorySecurity ds = Directory.GetAccessControl(folderPath);
            FileSystemAccessRule fsa = new FileSystemAccessRule(account, FileSystemRights.FullControl, AccessControlType.Deny);

            ds.RemoveAccessRule(fsa);

            Directory.SetAccessControl(folderPath, ds);
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
