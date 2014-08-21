using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            var assembly = this.GetArrangementsAssembly();
            Stream source = assembly.GetManifestResourceStream(FileResource);

            string filePath = this.GetDestinationFilePath(PackageName, LayoutFileName);
            Stream destination = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            
            CopyStream(source, destination);
        }

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
            ServerOperations.SystemManager().RestartApplication(true);

            string filePath = this.GetDestinationFilePath(PackageName, LayoutFileName);           
            File.Delete(filePath);
        }

        internal Assembly GetArrangementsAssembly()
        {
            var uiTestsArrangementsAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.Equals("Telerik.Sitefinity.Frontend.TestUI.Arrangements")).FirstOrDefault();
            if (uiTestsArrangementsAssembly == null)
            {
                throw new DllNotFoundException("Arrangements assembly wasn't found");
            }

            return uiTestsArrangementsAssembly;
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private string GetDestinationFilePath(string packageName, string layoutFileName)
        {
            var sfpath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            var filePath = Path.Combine(sfpath, "ResourcePackages", packageName, "MVC", "Views", "Layouts", layoutFileName);

            if (filePath == null)
            {
                throw new ArgumentNullException("FilePath doesn't exist");
            }

            return filePath;
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.TestLayout.cshtml";
        private const string PackageName = "Foundation";
        private const string LayoutFileName = "TestLayout.cshtml";
        private const string TemplateTitle = "Foundation.TestLayout";
    }
}
