using System.IO;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// AddNewLayoutFileToDefaultPackage arragement methods.
    /// </summary>
    public class AddNewLayoutFileToDefaultPackage : ITestArrangement
    {
        [ServerArrangement]
        public void AddNewLayoutFile()
        {
            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);
            FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);
        }

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Templates().DeletePageTemplate(TemplateTitle);

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageDestinationFilePath(PackageName, LayoutFileName);           
            File.Delete(filePath);
        } 
        
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayout.cshtml";
        private const string PackageName = "Foundation";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TemplateTitle = "TestLayout";
    }
}
