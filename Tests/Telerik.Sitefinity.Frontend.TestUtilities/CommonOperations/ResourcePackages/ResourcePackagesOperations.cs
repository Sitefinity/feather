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
        /// Gets the file path of the mvc view file from the resource package.
        /// </summary>
        /// <param name="packageName">The name of the package.</param>
        /// <param name="widgetName">The name of the Mvc widget.</param>
        /// <param name="viewFileName">The name of the view.</param>
        /// <returns></returns>
        public string GetResourcePackageMvcViewDestinationFilePath(string packageName, string widgetName, string viewFileName)
        {
            if (viewFileName == null)
                throw new ArgumentNullException("viewFileName");

            if (packageName == null)
                throw new ArgumentNullException("packageName");

            if (widgetName == null)
                throw new ArgumentNullException("widgetName");

            var folderPath = Path.Combine(this.SfPath, "ResourcePackages", packageName, "MVC", "Views", widgetName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, viewFileName);

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
                Thread.Sleep(500);
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(500);
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Imports the data for selectors tests.
        /// </summary>
        /// <param name="fileResource">The file resource.</param>
        /// <param name="designerViewFileName">Name of the designer view file.</param>
        /// <param name="fileResourceJson">The file resource json.</param>
        /// <param name="jsonFileName">Name of the json file.</param>
        /// <param name="controllerFileResource">The controller file resource.</param>
        /// <param name="controllerFileName">Name of the controller file.</param>
        public void ImportDataForSelectorsTests(string fileResource, string designerViewFileName, string fileResourceJson, string jsonFileName, string controllerFileResource, string controllerFileName)
        {
            var assembly = FileInjectHelper.GetArrangementsAssembly();

            ////  inject DesignerView.Selector.cshtml
            Stream source = assembly.GetManifestResourceStream(fileResource);

            var viewPath = Path.Combine("MVC", "Views", "DummyText", designerViewFileName);

            string filePath = FileInjectHelper.GetDestinationFilePath(viewPath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            Stream destination = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            FileInjectHelper.CopyStream(source, destination);
            source.Close();
            destination.Close();

            ////  inject DesignerView.Selector.json
            Stream sourceJson = assembly.GetManifestResourceStream(fileResourceJson);
            var jsonPath = Path.Combine("MVC", "Views", "DummyText", jsonFileName);

            string filePathJson = FileInjectHelper.GetDestinationFilePath(jsonPath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePathJson));
            Stream destinationJson = new FileStream(filePathJson, FileMode.Create, FileAccess.Write);

            FileInjectHelper.CopyStream(sourceJson, destinationJson);
            sourceJson.Close();
            destinationJson.Close();

            ////  inject designerview-selector.js
            Stream sourceController = assembly.GetManifestResourceStream(controllerFileResource);
            var controllerPath = Path.Combine("MVC", "Scripts", "DummyText", controllerFileName);

            string controllerFilePath = FileInjectHelper.GetDestinationFilePath(controllerPath);
            Directory.CreateDirectory(Path.GetDirectoryName(controllerFilePath));
            Stream destinationController = new FileStream(controllerFilePath, FileMode.Create, FileAccess.Write);

            FileInjectHelper.CopyStream(sourceController, destinationController);

            sourceController.Close();
            destinationController.Close();
        }

        /// <summary>
        /// Deletes the selectors data.
        /// </summary>
        public void DeleteSelectorsData(string designerViewFileName, string jsonFileName, string controllerFileName)
        {
            var path = Path.Combine("MVC", "Views", "DummyText", designerViewFileName);
            string filePath = FileInjectHelper.GetDestinationFilePath(path);
            File.Delete(filePath);

            var jsonPath = Path.Combine("MVC", "Views", "DummyText", jsonFileName);
            string filePathJson = FileInjectHelper.GetDestinationFilePath(jsonPath);
            File.Delete(filePathJson);

            var controllerPath = Path.Combine("MVC", "Scripts", "DummyText", controllerFileName);
            string controllerFilePath = FileInjectHelper.GetDestinationFilePath(controllerPath);
            File.Delete(controllerFilePath);
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

        /// <summary>
        /// Returns current Sitefinity intstance path.
        /// </summary>
        public string SfPath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/");
            }
        }
    }
}
