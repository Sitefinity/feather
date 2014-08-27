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
                FeatherServerOperations.ResourcePackages().AddNewLayoutFileToPackage(PackageName, LayoutFileName, FileResource);

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
                SystemManager.RestartApplication(true);

                string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
                File.Delete(filePath);
            }
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        private const string PackageName = "Foundation";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TemplateTitle = "Foundation.TestLayout";
    }
}
