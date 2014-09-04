using System;
using System.Collections.Generic;
using System.Globalization;
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
using Telerik.Sitefinity.Pages.Model;
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
                Directory.Delete(path, true);
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
                Directory.Delete(path, true);
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
                Directory.Delete(path, true);
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
