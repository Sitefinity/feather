﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.Resources;

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
        protected virtual IEnumerable<string> FolderPathStructure
        {
            get
            {
                if (this.folderPathStructure == null)
                    this.folderPathStructure = new string[] { "GridSystem", "Templates" };

                return this.folderPathStructure;
            }
        }

        /// <summary>
        /// Gets the widget registrator.
        /// </summary>
        /// <value>
        /// The widget registrator.
        /// </value>
        protected virtual GridWidgetRegistrator WidgetRegistrator
        {
            get
            {
                return new GridWidgetRegistrator();
            }
        }

        #endregion

        #region IFileManager

        /// <summary>
        /// Process the file if such is added to the existing folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileData">The file data.</param>
        /// <param name="packageName">Name of the package.</param>
        public void FileAdded(string fileName, string filePath, FileData fileData, string packageName = "")
        {
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();

            if (this.AddToToolboxAndFileData(fileMonitorDataManager, fileName, filePath, packageName, fileData))
            {
                fileMonitorDataManager.SaveChanges();
            }
        }

        /// <summary>
        /// Called on file deletion
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        public void FileDeleted(string filePath, string packageName)
        {
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();
            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (fileData != null)
            {
                fileMonitorDataManager.Delete(fileData);
                fileMonitorDataManager.SaveChanges();
            }

            this.WidgetRegistrator.UnregisterToolboxItem(this.GetFileName(filePath));
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
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();
            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(oldFilePath, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (fileData != null)
            {
                fileData.FilePath = newFilePath;
                if (this.AddToToolboxAndFileData(fileMonitorDataManager, newFileName, newFilePath, packageName, fileData, oldFileName))
                {
                    this.WidgetRegistrator.UpdateControlData(newFileName, oldFileName);
                    fileMonitorDataManager.SaveChanges();
                }
                else
                {
                    fileMonitorDataManager.Delete(fileData);
                    fileMonitorDataManager.SaveChanges();
                }
            }
            else
            {
                this.FileAdded(newFileName, newFilePath, null, packageName);
            }
        }

        #endregion

        #region Private methods

        private bool AddToToolboxAndFileData(FileMonitorDataManager fileMonitorDataManager, string newFileName, string newFilePath, string packageName, FileData fileData, string oldFileName = "")
        {
            if (!this.IsFileValid(newFileName, newFilePath, packageName))
                return false;

            this.WidgetRegistrator.RegisterToolboxItem(newFileName, oldFileName);

            return this.CreateOrUpdateFileData(fileMonitorDataManager, newFileName, newFilePath, packageName, fileData);
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

            var resourcePackagesPath = FrontendManager.VirtualPathBuilder.MapPath(string.Concat("~/", PackageManager.PackagesFolder));
            if (directory.FullName.EndsWith(expectedGridFolderStructure, StringComparison.OrdinalIgnoreCase) &&
                    (
                        directory.FullName.StartsWith(HostingEnvironment.ApplicationPhysicalPath, StringComparison.OrdinalIgnoreCase) ||
                        directory.FullName.StartsWith(resourcePackagesPath, StringComparison.OrdinalIgnoreCase)
                    )
                )
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

            var isValid = isSupproted && this.IsFileInValidFolder(absolutePath, packageName);

            return isValid;
        }

        private bool CreateOrUpdateFileData(FileMonitorDataManager dataManager, string fileName, string filePath, string packageName, FileData fileData)
        {
            var changed = false;
            if (fileData == null)
            {
                fileData = dataManager.CreateFileData();
                fileData.FilePath = filePath;
                changed = true;
            }

            if (fileData.FileName != fileName)
            {
                fileData.FileName = fileName;
                changed = true;
            }

            if (fileData.PackageName != packageName)
            {
                fileData.PackageName = packageName;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private string GetFileName(string filePath)
        {
            var fileName = filePath.Substring(filePath.LastIndexOf('/') + 1);

            return fileName;
        }

        #endregion

        #region Private fileds and constants

        /// <summary>
        /// The folder path structure
        /// </summary>
        private string[] folderPathStructure;

        /// <summary>
        /// Allowed extension for grid template files.
        /// </summary>
        internal const string GridTemplateExtension = "html";

        #endregion
    }
}
