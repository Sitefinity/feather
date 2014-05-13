﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
using Telerik.Sitefinity.Modules.Pages;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// This class manages the behavior when a layout file is moved over the application folder structure.
    /// </summary>
    public class LayoutFileManager : IFileManager
    {
        #region Properties

        /// <summary>
        /// Gets the required folder path structure. Only layout files placed inside the specified folder structure will trigger automatic creation of the templates.
        /// </summary>
        /// <value>
        /// The folder path structure.
        /// </value>
        public virtual List<string> FolderPathStructure
        {
            get
            {
                if (this.folderPathStructure == null)
                    this.folderPathStructure = new List<string> { "Mvc", "Views" };

                return this.folderPathStructure;
            }
        }

        #endregion

        #region IFileManager

        /// <summary>
        /// Process the file if such is added to the existing folder.
        /// </summary>
        /// <param name="virtualFilePath">The virtual file path.</param>
        /// <param name="packageName">Name of the package.</param>
        public void FileAdded(string fileName, string filePath, string packageName = "")
        {
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            this.CreateTemplateAndFileData(fileName, filePath, packageName, fileMonitorDataManager, fileData);
        }

        /// <summary>
        /// Called on file deletion
        /// </summary>
        /// <param name="path">The file path.</param>
        public void FileDeleted(string filePath)
        {
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (fileData != null)
            {
                fileMonitorDataManager.Delete(fileData);
                fileMonitorDataManager.SaveChanges();
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
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(oldFilePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (fileData != null)
                this.CreateTemplateAndFileData(newFileName, newFilePath, packageName, fileMonitorDataManager, fileData);
            else
                this.FileAdded(newFileName, newFilePath, packageName);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Determines whether a file exists on the specified location and whether it is applicable for the current application.
        /// </summary>
        /// <remarks>
        /// Valid locations depends on the values inside the <see cref="FolderPathStructure"/>.
        /// </remarks>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns>true if the file exists and is placed under the application root in a folder [package]/Mvc/Views/Layouts.</returns>
        private bool IsFileInValidFolder(string filePath, string packageName = "")
        {
            bool isFileInValidFolder = false;

            FileInfo info = new FileInfo(filePath);

            if (!info.Exists)
                return false;

            var directory = info.Directory;

            var expectedBaseFolderStructure = string.Join(Path.DirectorySeparatorChar.ToString(), this.FolderPathStructure);
            var expectedLayoutFolderStructure = expectedBaseFolderStructure + Path.DirectorySeparatorChar + ResourceType.Layouts.ToString();

            if (!string.IsNullOrEmpty(packageName))
                expectedLayoutFolderStructure = packageName + Path.DirectorySeparatorChar + expectedLayoutFolderStructure;

            if (directory.FullName.EndsWith(expectedLayoutFolderStructure, StringComparison.InvariantCultureIgnoreCase)
                && directory.FullName.StartsWith(HostingEnvironment.ApplicationPhysicalPath, StringComparison.InvariantCultureIgnoreCase))
                isFileInValidFolder = true;

            return isFileInValidFolder;
        }

        /// <summary>
        /// Creates the template and file data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="fileMonitorDataManager">The file monitor data manager.</param>
        /// <param name="fileData">The file data.</param>
        private void CreateTemplateAndFileData(string fileName, string filePath, string packageName, FileMonitorDataManager fileMonitorDataManager, FileData fileData)
        {
            if (!this.IsFileInValidFolder(filePath, packageName))
                return;

            var extension = fileName.Split('.').LastOrDefault();
            var fileNameWithoutExtension = fileName.Substring(0, fileName.Length - (extension.Length + 1));

            var viewFileExtensions = this.GetViewExtensions();

            if (viewFileExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
            {
                string templateTitle = string.Empty;

                if (string.IsNullOrEmpty(packageName))
                    templateTitle = fileNameWithoutExtension;
                else
                    templateTitle = packageName + "." + fileNameWithoutExtension;

                if (fileData == null)
                    fileData = fileMonitorDataManager.CreateFileData();

                fileData.FilePath = filePath;
                fileData.FileName = fileName;
                fileData.PackageName = packageName;

                fileMonitorDataManager.SaveChanges();

                this.CreateTemplate(templateTitle);
            }
        }

        /// <summary>
        /// Gets the allowed views extensions.
        /// </summary>
        /// <returns></returns>
        private string[] GetViewExtensions()
        {
            return ViewEngines.Engines.OfType<VirtualPathProviderViewEngine>()
                .SelectMany(p => p.FileExtensions).ToArray();
        }

        /// <summary>
        /// Creates the page template.
        /// </summary>
        /// <param name="templateTitle">The template title.</param>
        private void CreateTemplate(string templateTitle)
        {
            PageManager pageManager = PageManager.GetManager();

            using (new ElevatedModeRegion(pageManager))
            {
                if (pageManager.GetTemplates().Where(pt => pt.Title.Equals(templateTitle, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
                {
                    var template = pageManager.CreateTemplate();
                    template.Category = SiteInitializer.CustomTemplatesCategoryId;
                    template.Name = templateTitle;
                    template.Title = templateTitle;
                    template.Framework = Pages.Model.PageTemplateFramework.Mvc;

                    var languageData = pageManager.CreatePublishedInvarianLanguageData();
                    template.LanguageData.Add(languageData);

                    pageManager.SaveChanges();

                    var master = pageManager.TemplatesLifecycle.Edit(template);
                    pageManager.TemplatesLifecycle.Publish(master);
                    pageManager.SaveChanges();
                }
            }
        }

        #endregion

        #region Private fileds

        private List<string> folderPathStructure;

        #endregion
    }
}
