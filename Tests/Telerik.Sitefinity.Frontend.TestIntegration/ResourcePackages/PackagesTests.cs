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
        [Category(TestCategories.Samples)]
        [Author("Petya Rachina")]
        public void FoundationResourcePackage_AddNewLayoutFile_VerifyGeneratedTemplate()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            try
            {
                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
                FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);

                for (int i = 50; i > 0; --i)
                {
                    if (pageManager.GetTemplates().Count() == templatesCount + 1)
                        break;

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
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
        [Category(TestCategories.Samples)]
        [Author("Petya Rachina")]
        public void ResourcePackage_AddNewPackageWithLayoutFiles_VerifyGeneratedTemplates()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            try
            {
                var sfpath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                var path = Path.Combine(sfpath, "ResourcePackages");

                var assembly = FeatherServerOperations.ResourcePackages().GetTestUtilitiesAssembly();
                Stream source = assembly.GetManifestResourceStream(PackageResource);

                byte[] data = new byte[source.Length];

                source.Read(data, 0, (int)source.Length);

                using (var stream = new MemoryStream(data))
                {
                    using (ZipFile zipFile = ZipFile.Read(stream))
                    {
                        zipFile.ExtractAll(path, true);
                    }
                }

                for (int i = 50; i > 0; --i)
                {
                    if (pageManager.GetTemplates().Count() == templatesCount + 3)
                        break;

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                string[] templates = new string[] { TestLayout1, TestLayout2, TestLayout3 };

                foreach (var template in templates)
                {
                    pageManager.GetTemplates().Where(t => t.Title == template).FirstOrDefault();
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

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        private const string PackageResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestPackage.zip";
        private const string PackageName = "Foundation";
        private const string TestPackageName = "TestPackage";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TemplateTitle = "Foundation.TestLayout";
        private const string TestLayout1 = "TestPackage.Test Layout 1";
        private const string TestLayout2 = "TestPackage.Test Layout 2";
        private const string TestLayout3 = "TestPackage.Test Layout 3";
    }
}
