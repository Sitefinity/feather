using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations
{
    /// <summary>
    /// Provides common resource packages operations
    /// </summary>
    public class ResourcePackagesOperations
    {
        /// <summary>
        /// Gets the file path of the layout file from the resource package.
        /// </summary>
        /// <param name="packageName">The name of the package.</param>
        /// <param name="layoutFileName">The layout file name.</param>
        /// <returns>The file path if exists.</returns>
        public string GetResourcePackageDestinationFilePath(string packageName, string layoutFileName)
        {
            var sfpath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            var filePath = Path.Combine(sfpath, "ResourcePackages", packageName, "MVC", "Views", "Layouts", layoutFileName);

            if (filePath == null)
            {
                throw new ArgumentNullException("FilePath was not found!");
            }

            return filePath;
        }

        /// <summary>
        /// Gets the file path of the Resource Packages folder
        /// </summary>
        /// <returns>The file path if exists.</returns>
        public string GetResourcePackagesDestination(string packageName)
        {
            var sfpath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            var packagePath = Path.Combine(sfpath, "ResourcePackages", packageName);

            if (packagePath == null)
            {
                throw new ArgumentNullException("FilePath was not found!");
            }

            return packagePath;
        }

        /// <summary>
        /// Adds new layout file to a selected resource package.
        /// </summary>
        /// <param name="packageName">The name of the package.</param>
        /// <param name="layoutFileName">The name of the layout file.</param>
        /// <param name="fileResource">The file resource.</param>
        public void AddNewResource(string fileResource, string filePath)
        {
            var assembly = this.GetTestUtilitiesAssembly();
            Stream source = assembly.GetManifestResourceStream(fileResource);

            Stream destination = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            this.CopyStream(source, destination);

            destination.Dispose();
        }

        /// <summary>
        /// Gets test UI arrangements assebly.
        /// </summary>
        /// <returns>The UI tests arrangements assembly.</returns>
        public Assembly GetTestUtilitiesAssembly()
        {
            var uiTestsArrangementsAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.Equals("Telerik.Sitefinity.Frontend.TestUtilities")).FirstOrDefault();
            if (uiTestsArrangementsAssembly == null)
            {
                throw new DllNotFoundException("Assembly wasn't found");
            }

            return uiTestsArrangementsAssembly;
        }

        /// <summary>
        /// Copies file stream to another file stream
        /// </summary>
        /// <param name="input">The input file.</param>
        /// <param name="output">The destination file.</param>
        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
