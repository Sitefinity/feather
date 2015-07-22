using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    public class ApplyTwoDifferentTemplatesFromDifferentPackagesToPages : ITestArrangement
    {
        [ServerSetUp]
        public void Setup()
        {
            FeatherServerOperations.ResourcePackages().AddNewResourcePackage(Package1Resource);
            FeatherServerOperations.ResourcePackages().AddNewResourcePackage(Package2Resource);          
        }

        [ServerArrangement]
        public void CreatePages()
        {
            this.CreatePageWithTemplate(Template1Title, Page1Title, Page1Url);
            this.CreatePageWithTemplate(Template2Title, Page2Title, Page2Url);
        }

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
            ServerOperations.Templates().DeletePageTemplate(Template1Title);
            ServerOperations.Templates().DeletePageTemplate(Template2Title);

            string path1 = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(Package1Name);
            FeatherServerOperations.ResourcePackages().DeleteDirectory(path1);

            string path2 = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(Package2Name);
            FeatherServerOperations.ResourcePackages().DeleteDirectory(path2);          
        }

        private void CreatePageWithTemplate(string templateName, string pageName, string pageUrl)
        {
            PageManager pageManager = PageManager.GetManager();

            var template = pageManager.GetTemplates().Where(t => t.Title == templateName).FirstOrDefault();

            if (template == null)
            {
                throw new ArgumentException("template is null");
            }

            FeatherServerOperations.Pages().CreatePageWithTemplate(template, pageName, pageUrl);
        }

        private const string Package1Resource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Package11.zip";
        private const string Package2Resource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Package22.zip";
        private const string Package1Name = "Package11";
        private const string Package2Name = "Package22";
        private const string Template1Title = "Package11.test-layout";
        private const string Template2Title = "Package22.test-layout";
        private const string Page1Title = "page1";
        private const string Page2Title = "page2";
        private const string Page1Url = "page1";
        private const string Page2Url = "page2";
    }
}
