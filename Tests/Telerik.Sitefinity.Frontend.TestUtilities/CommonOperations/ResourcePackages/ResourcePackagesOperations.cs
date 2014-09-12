using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Utilities.Zip;

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
            if (layoutFileName == null)
                throw new ArgumentNullException("layoutFileName");

            if (packageName == null)
                throw new ArgumentNullException("packageName");

            var filePath = Path.Combine(this.SfPath, "ResourcePackages", packageName, "MVC", "Views", "Layouts", layoutFileName);

            if (filePath == null)
                throw new ArgumentException("filePath was not found");

            return filePath;
        }

        /// <summary>
        /// Gets the file path of the Resource Packages folder
        /// </summary>
        /// <returns>The file path if exists.</returns>
        public string GetResourcePackagesDestination(string packageName)
        {
            var packagePath = Path.Combine(this.SfPath, "ResourcePackages", packageName);

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
        /// Gets Test Utilities Assembly.
        /// </summary>
        /// <returns>The Test Utilities Assembly.</returns>
        public Assembly GetTestUtilitiesAssembly()
        {
            var testUtilitiesAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.Equals("Telerik.Sitefinity.Frontend.TestUtilities")).FirstOrDefault();
            if (testUtilitiesAssembly == null)
            {
                throw new DllNotFoundException("Assembly wasn't found");
            }

            return testUtilitiesAssembly;
        }

        /// <summary>
        /// Renames the resource package folder.
        /// </summary>
        /// <param name="packageName">The name of the package.</param>
        /// <param name="newPackageName">The new name of the package.</param>
        public void RenamePackageFolder(string packageName, string newPackageName)
        {
            var packagePath = this.GetResourcePackagesDestination(packageName);

            var directoryPath = Path.Combine(this.SfPath, "ResourcePackages", newPackageName);

            Directory.Move(packagePath, directoryPath);
        }

        /// <summary>
        /// Renames layout file from specific resource package.
        /// </summary>
        /// <param name="packageName">The package name.</param>
        /// <param name="layoutFileName">The layout file name.</param>
        /// <param name="newLayoutFileName">The new name of the layout file.</param>
        public void RenameLayoutFile(string packageName, string layoutFileName, string newLayoutFileName)
        {
            var layoutFilePath = this.GetResourcePackageDestinationFilePath(packageName, layoutFileName);

            var newLayoutFilePath = Path.Combine(this.SfPath, "ResourcePackages", packageName, "MVC", "Views", "Layouts", newLayoutFileName);

            File.Move(layoutFilePath, newLayoutFilePath);
        }

        /// <summary>
        /// Adds new resource package to file system.
        /// </summary>
        /// <param name="packageResource">The package resource path.</param>
        public void AddNewResourcePackage(string packageResource)
        {
            var path = Path.Combine(this.SfPath, "ResourcePackages");

            var assembly = FeatherServerOperations.ResourcePackages().GetTestUtilitiesAssembly();
            Stream source = assembly.GetManifestResourceStream(packageResource);

            byte[] data = new byte[source.Length];

            source.Read(data, 0, (int)source.Length);

            using (var stream = new MemoryStream(data))
            {
                using (ZipFile zipFile = ZipFile.Read(stream))
                {
                    zipFile.ExtractAll(path, true);
                }
            }
        }

        /// <summary>
        /// Waits for templates count to increase with some number.
        /// </summary>
        /// <param name="primaryCount">The primary templates count.</param>
        /// <param name="increment">The increase number.</param>
        public void WaitForTemplatesCountToIncrease(int primaryCount, int increment)
        {
            PageManager pageManager = PageManager.GetManager();

            for (int i = 50; i > 0; --i)
            {
                if (pageManager.GetTemplates().Count() == primaryCount + increment)
                    break;

                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        }

        /// <summary>
        /// Edit a layout file content
        /// </summary>
        /// <param name="layoutFile">The layout file path.</param>
        /// <param name="textToReplace">The text you want to replace.</param>
        /// <param name="newText">The new text you want to add.</param>
        public void EditLayoutFile(string layoutFile, string textToReplace, string newText)
        {
            string text = File.ReadAllText(layoutFile);
            text = text.Replace(textToReplace, newText);
            File.WriteAllText(layoutFile, text);
        }

        /// <summary>
        /// Deletes directory from the file system
        /// </summary>
        /// <param name="path">The directory path.</param>
        public void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(50);
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(50);
                Directory.Delete(path, true);
            }
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

        private string SfPath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/");
            }
        }
    }
}
