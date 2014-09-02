using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using MbUnit.Framework;
using Microsoft.Http;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Utilities.Zip;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.ResourcePackages
{
    [TestFixture]
    [Description("This is a class with tests related to feather resource packages.")]
    public class PackagesTests
    {
        [Test]
        [Category(TestCategories.Packages)]
        [Author("Petya Rachina")]
        public void FoundationResourcePackage_AddNewLayoutFile_VerifyGeneratedTemplate()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
                FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);

                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

                var template = this.PageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");
            }
            finally
            {
                ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
                File.Delete(filePath);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author("Petya Rachina")]
        public void ResourcePackage_AddNewPackageWithLayoutFiles_VerifyGeneratedTemplates()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(PackageResource);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 3);

                string[] templateTitles = new string[] { TestLayout1, TestLayout2, TestLayout3 };

                foreach (var title in templateTitles)
                {
                    var template = this.PageManager.GetTemplates().Where(t => t.Title == title).FirstOrDefault();
                    Assert.IsNotNull(template, "Template was not found");
                }               
            }
            finally
            {
                string[] templates = new string[] { TestLayout1, TestLayout2, TestLayout3 };

                foreach (var template in templates)
                {
                    ServerOperations.Templates().DeletePageTemplate(template);
                }

                string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(TestPackageName);
                Directory.Delete(path, true);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author("Petya Rachina")]
        public void ResourcePackage_RenamePackageWithLayoutFiles_VerifyGeneratedTemplates()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(PackageResource);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 3);

                string[] templateTitles = new string[] { TestLayout1, TestLayout2, TestLayout3 };

                foreach (var title in templateTitles)
                {
                    var template = this.PageManager.GetTemplates().Where(t => t.Title == title).FirstOrDefault();
                    Assert.IsNotNull(template, "Template was not found");
                }

                FeatherServerOperations.ResourcePackages().RenamePackageFolder(TestPackageName, NewTestPackageName);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 6);

                string[] newTemplateTitles = new string[] { NewTestLayout1, NewTestLayout2, NewTestLayout3 };

                foreach (var title in newTemplateTitles)
                {
                    var template = this.PageManager.GetTemplates().Where(t => t.Title == title).FirstOrDefault();
                    Assert.IsNotNull(template, "Template was not found");
                }
            }
            finally
            {
                string[] templates = new string[] { TestLayout1, TestLayout2, TestLayout3, NewTestLayout1, NewTestLayout2, NewTestLayout3 };

                foreach (var template in templates)
                {
                    ServerOperations.Templates().DeletePageTemplate(template);
                }

                string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(NewTestPackageName);
                Directory.Delete(path, true);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author("Petya Rachina")]
        public void ResourcePackage_RenameLayoutFile_VerifyGeneratedTemplate()
        {
            int templatesCount = this.PageManager.GetTemplates().Count();

            try
            {
                FeatherServerOperations.ResourcePackages().AddNewResourcePackage(PackageResource);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 3);

                string[] templateTitles = new string[] { TestLayout1, TestLayout2, TestLayout3 };

                foreach (var title in templateTitles)
                {
                    var template = this.PageManager.GetTemplates().Where(t => t.Title == title).FirstOrDefault();
                    Assert.IsNotNull(template, "Template was not found");
                }

                FeatherServerOperations.ResourcePackages().RenameLayoutFile(TestPackageName, TestLayoutFileName, NewTestLayoutFileName);
                FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 4);

                var renamedTemplate = this.PageManager.GetTemplates().Where(t => t.Title == NewTestLayout1Renamed).FirstOrDefault();
                Assert.IsNotNull(renamedTemplate, "Template was not found");
            }
            finally
            {
                string[] templates = new string[] { TestLayout1, TestLayout2, TestLayout3, NewTestLayout1Renamed };

                foreach (var template in templates)
                {
                    ServerOperations.Templates().DeletePageTemplate(template);
                }

                string path = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(TestPackageName);
                Directory.Delete(path, true);
            }
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        private const string PackageResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestPackage.zip";
        private const string PackageName = "Foundation";
        private const string TestPackageName = "TestPackage";
        private const string NewTestPackageName = "NewTestPackage";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TestLayoutFileName = "Test Layout 1.cshtml";
        private const string NewTestLayoutFileName = "New Test Layout 1.cshtml";
        private const string TemplateTitle = "Foundation.TestLayout";
        private const string TestLayout1 = "TestPackage.Test Layout 1";
        private const string TestLayout2 = "TestPackage.Test Layout 2";
        private const string TestLayout3 = "TestPackage.Test Layout 3";
        private const string NewTestLayout1 = "NewTestPackage.Test Layout 1";
        private const string NewTestLayout2 = "NewTestPackage.Test Layout 2";
        private const string NewTestLayout3 = "NewTestPackage.Test Layout 3";
        private const string NewTestLayout1Renamed = "TestPackage.New Test Layout 1";

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
