using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Multisite;
using Telerik.Sitefinity.Project.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// This class manages the behavior when a grid file is moved over the application folder structure.
    /// </summary>
    internal class GridFileManager : IFileManager
    {
        #region Properties

        /// <summary>
        /// Gets the required folder path structure. Only grid files placed inside the specified folder structure will trigger automatic creation of the templates.
        /// </summary>
        /// <value>
        /// The folder path structure.
        /// </value>
        public virtual ICollection<string> FolderPathStructure
        {
            get
            {
                if (this.folderPathStructure == null)
                    this.folderPathStructure = new List<string> { "GridSystem", "Templates" };

                return this.folderPathStructure;
            }
        }

        /// <summary>
        /// Gets the widget registrator.
        /// </summary>
        /// <value>
        /// The widget registrator.
        /// </value>
        public virtual GridWidgetRegistrator WidgetRegistrator
        {
            get
            {
                return new GridWidgetRegistrator();
            }
        }

        /// <summary>
        /// Gets the file monitor data manager.
        /// </summary>
        /// <value>
        /// The file monitor data manager.
        /// </value>
        public virtual FileMonitorDataManager FileMonitorManager
        {
            get 
            {
                if (this.fileMonitorDataManager == null)
                    this.fileMonitorDataManager = FileMonitorDataManager.GetManager();

                return this.fileMonitorDataManager;
            }
        }

        #endregion

        #region IFileManager

        /// <summary>
        /// Process the file if such is added to the existing folder.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <param name="packageName">Name of the package.</param>
        public void FileAdded(string fileName, string filePath, string packageName = "")
        {
            var fileData = this.FileMonitorManager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            this.AddToToolboxAndFileData(fileName, filePath, packageName, fileData);
        }

        /// <summary>
        /// Called on file deletion
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void FileDeleted(string filePath)
        {
            var fileData = this.FileMonitorManager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (fileData != null)
            {
                this.FileMonitorManager.Delete(fileData);
                this.FileMonitorManager.SaveChanges();
            }
        }

        /// <summary>
        /// Reacts on file renaming
        /// </summary>
        /// <param name="newFileName">New name of the file.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        /// <param name="newFilePath"></param>
        /// <param name="oldFilePath"></param>
        /// <param name="packageName">Name of the package.</param>
        public void FileRenamed(string newFileName, string oldFileName, string newFilePath, string oldFilePath, string packageName = "")
        {
            var fileData = this.FileMonitorManager.GetFilesData().Where(file => file.FilePath.Equals(oldFilePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            this.AddToToolboxAndFileData(newFileName, newFilePath, packageName, fileData, oldFileName);
        }

        #endregion

        #region Private methods

        private void AddToToolboxAndFileData(string newFileName, string newFilePath, string packageName, FileData fileData, string oldFileName = "")
        {
            if (!this.IsFileValid(newFileName, newFilePath, packageName))
                return;

            this.WidgetRegistrator.RegisterInToolbox(newFileName, packageName, oldFileName);
            this.CreateOrUpdateFileData(newFileName, newFilePath, packageName, fileData);
        }

        /// <summary>
        /// Determines whether a file exists on the specified location and whether it is applicable for the current application.
        /// </summary>
        /// <remarks>
        /// Valid locations depends on the values inside the <see cref="FolderPathStructure"/>.
        /// </remarks>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns>true if the file exists and is placed under the application root in a folder [package]/GridSystem/Templates/ .</returns>
        private bool IsFileInValidFolder(string filePath, string packageName = "")
        {
            bool isFileInValidFolder = false;

            FileInfo info = new FileInfo(filePath);

            if (!info.Exists)
                return false;

            var directory = info.Directory;

            var expectedGridFolderStructure = string.Join(Path.DirectorySeparatorChar.ToString(), this.FolderPathStructure);

            if (!string.IsNullOrEmpty(packageName))
                expectedGridFolderStructure = expectedGridFolderStructure.Insert(0, packageName + Path.DirectorySeparatorChar);

            if (directory.FullName.EndsWith(expectedGridFolderStructure, StringComparison.OrdinalIgnoreCase) && directory.FullName.StartsWith(HostingEnvironment.ApplicationPhysicalPath, StringComparison.OrdinalIgnoreCase))
                isFileInValidFolder = true;

            return isFileInValidFolder;
        }

        /// <summary>
        /// Determines whether specified file is exist and is eligible for registration.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns></returns>
        private bool IsFileValid(string fileName, string filePath, string packageName)
        {
            var extension = fileName.Split('.').LastOrDefault();
            var absolutePath = FrontendManager.VirtualPathBuilder.MapPath(filePath);
            var isSupproted = extension == GridFileManager.GridTemplateExtension;

            var isValid = isSupproted && !packageName.IsNullOrEmpty() && this.IsFileInValidFolder(absolutePath, packageName);

            return isValid;
        }

        /// <summary>
        /// Creates the or update file data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="fileData">The file data.</param>
        private void CreateOrUpdateFileData(string fileName, string filePath, string packageName, FileData fileData)
        {
            if (fileData == null)
                fileData = this.FileMonitorManager.CreateFileData();

            fileData.FilePath = filePath;
            fileData.FileName = fileName;
            fileData.PackageName = packageName;

            this.FileMonitorManager.SaveChanges();
        }

        #endregion

        #region Private fileds and constants

        /// <summary>
        /// The folder path structure
        /// </summary>
        private List<string> folderPathStructure;

        /// <summary>
        /// Allowed extension for grid template files.
        /// </summary>
        internal const string GridTemplateExtension = "html";

        private FileMonitorDataManager fileMonitorDataManager;

        #endregion
    }
}
