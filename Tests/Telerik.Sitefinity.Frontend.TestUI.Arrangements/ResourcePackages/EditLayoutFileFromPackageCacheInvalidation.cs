using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    public class EditLayoutFileFromPackageCacheInvalidation : TestArrangementBase
    {
        [ServerSetUp]
        public void AddNewLayoutFile()
        {
            AuthenticationHelper.AuthenticateUser(AdminEmail, AdminPass);

            PageManager manager = PageManager.GetManager();
            int templatesCount = manager.GetTemplates().Count();

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
            FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);

            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);
            ServerOperations.Templates().SharePageTemplateWithSite(TemplateTitle, "SecondSite");

            Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(TemplateTitle);
            ServerOperations.Pages().CreatePage(PageTitle, templateId);            
        }

        [ServerArrangement]
        public void EditLayoutFile()
        {
            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
            FeatherServerOperations.ResourcePackages().EditLayoutFile(filePath, LayoutFileText, LayoutFileNewText);
        }

        [ServerTearDown]
        public void TearDown()
        {
            AuthenticationHelper.AuthenticateUser(AdminEmail, AdminPass);

            ServerOperations.Pages().DeleteAllPages();
            ServerOperations.Templates().UnSharePageTemplateWithSite(TemplateTitle, "SecondSite");
            ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
            File.Delete(filePath);
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        private const string PackageName = "Foundation";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TemplateTitle = "TestLayout";
        private const string PageTitle = "FeatherPage";
        private const string LayoutFileText = "Test Layout";
        private const string LayoutFileNewText = "New Text";
        private const string AdminEmail = "admin@test.test";
        private const string AdminPass = "admin@2";
    }
}
