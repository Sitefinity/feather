using System.IO;
using System.Linq;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// AddNewLayoutFileToDefaultPackage arragement methods.
    /// </summary>
    public class AddNewLayoutFileToDefaultPackage : TestArrangementBase
    {
        [ServerArrangement]
        public void AddNewLayoutFile()
        {
            AuthenticationHelper.AuthenticateUser(AdminEmail, AdminPass, true);
            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
            FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();
            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);
            ServerOperations.Templates().SharePageTemplateWithSite(TemplateTitle, "SecondSite");
        }

        [ServerTearDown]
        public void TearDown()
        {
            AuthenticationHelper.AuthenticateUser(AdminEmail, AdminPass, true);

            ServerOperations.Templates().UnSharePageTemplateWithSite(TemplateTitle, "SecondSite");
            ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);           
            File.Delete(filePath);
        } 
        
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        private const string PackageName = "Foundation";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TemplateTitle = "TestLayout";
        private const string AdminEmail = "admin@test.test";
        private const string AdminPass = "admin@2";
    }
}
