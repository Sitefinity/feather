using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.Sitefinity.Frontend.TestUtilities;
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
            //// Path.Combine(sfpath, "ResourcePackages", packageName, "MVC", "Views", "Layouts", layoutFileName);

            var assembly = FileInjectHelper.GetArrangementsAssembly();
            Stream source = assembly.GetManifestResourceStream(FileResource);

            var path = Path.Combine("ResourcePackages",PackageName, "MVC", "Views", "Layouts", LayoutFileName);

            string filePath = FileInjectHelper.GetDestinationFilePath(path);
            Stream destination = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            FileInjectHelper.CopyStream(source, destination);
        }

        [ServerTearDown]
        public void TearDown()
        {
            var path = Path.Combine("ResourcePackages", PackageName, "MVC", "Views", "Layouts", LayoutFileName);
            string filePath = FileInjectHelper.GetDestinationFilePath(path);

            ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
            File.Delete(filePath); 
        }

      
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.TestLayout.cshtml";
        private const string PackageName = "Foundation";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TemplateTitle = "Foundation.TestLayout";
    }
}
