using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    public class LayoutFilesManager : IFileManager
    {
        #region Properties

        /// <summary>
        /// Gets the required folder path structure.
        /// </summary>
        /// <value>
        /// The folder path structure.
        /// </value>
        public virtual List<string> FolderPathStructure
        {
            get
            {
                if (this.folderPathStructure == null)
                {
                    this.folderPathStructure = new List<string> { "Mvc", "Views" };
                }
                return this.folderPathStructure;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Determines whether [the file is in valid folder].
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns></returns>
        protected virtual bool IsFileInValidFolder(string filePath, string packageName = "")
        {
            bool isFileInValidFolder = false;

            FileInfo info = new FileInfo(filePath);
            //continue only if the folder exists
            if (info.Exists)
            {
                var directory = info.Directory;

                //The immediate folder containing the file must match the resource type 
                if (directory.Name.Equals(ResourceTypes.Layouts.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    bool canContinue = true;

                    int folderPosition = this.FolderPathStructure.Count;

                    //Loop trough the required folder structure and compere it with the actual folder structure
                    while (folderPosition != 0)
                    {
                        if (directory.Parent == null)
                        {
                            canContinue = false;
                            break;
                        }

                        directory = directory.Parent;

                        var folder = this.FolderPathStructure[folderPosition - 1];

                        if (directory.Name.Equals(folder, StringComparison.InvariantCultureIgnoreCase) == false)
                        {
                            canContinue = false;
                            break;
                        }

                        folderPosition = folderPosition - 1;
                    }

                    //If the package name is not empty then check if the actual folder structure contains the package folder and the packages container folder
                    if (string.IsNullOrEmpty(packageName) == false)
                    {
                        if (directory.Parent != null && directory.Parent.Parent != null)
                        {
                            if (directory.Parent.Name.Equals(packageName, StringComparison.InvariantCultureIgnoreCase) == false
                                || directory.Parent.Parent.Name.Equals(PackagesManager.PackagesFolder, StringComparison.InvariantCultureIgnoreCase) == false)
                            {
                                canContinue = false;
                            }

                            directory = directory.Parent.Parent;
                        }
                        else
                        {
                            canContinue = false;
                        }
                    }

                    //The application root must be the root of the actual folder structure
                    if (canContinue)
                    {
                        if (directory.Parent != null)
                        {
                            string rootDirectory = directory.Parent.FullName + "\\";

                            if (rootDirectory.Equals(HostingEnvironment.ApplicationPhysicalPath, StringComparison.InvariantCultureIgnoreCase))
                                isFileInValidFolder = true;
                        }
                    }
                }
            }
            return isFileInValidFolder;
        }

        #endregion

        #region IFileManager
        /// <summary>
        /// Process the file if such is added to the folder.
        /// </summary>
        /// <param name="virtualFilePath">The virtual file path.</param>
        /// <param name="packageName">Name of the package.</param>
        public void FileAdded(string fileName, string filePath, string packageName = "")
        {
            if (this.IsFileInValidFolder(filePath, packageName))
            {
                var fileMonitorDatamanager = FileMonitorDataManager.GetManager();

                var fileData = fileMonitorDatamanager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if (fileData == null)
                {
                    string templateName = string.Empty;

                    var extension = fileName.Split('.').LastOrDefault();

                    var fileNameWithoutExtension = fileName.Substring(0, fileName.Length - (extension.Length + 1));

                    var viewFileExtensions = this.GetViewExtensions();

                    if (viewFileExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(packageName))
                        {
                            templateName = fileNameWithoutExtension;
                        }
                        else
                        {
                            templateName = packageName + "." + fileNameWithoutExtension;
                        }

                        fileData = fileMonitorDatamanager.CreateFileData();

                        fileData.FilePath = filePath;
                        fileData.FileName = fileName;
                        fileData.PackageName = packageName;

                        fileMonitorDatamanager.SaveChanges();

                        this.CreateTemplate(templateName);
                    }
                }
            }
        }

        /// <summary>
        /// Called on file deletion
        /// </summary>
        /// <param name="path">The file path.</param>
        public void FileDeleted(string filePath)
        {
            var fileMonitorDatamanager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDatamanager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (fileData != null)
            {
                fileMonitorDatamanager.Delete(fileData);
                fileMonitorDatamanager.SaveChanges();
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
            var fileMonitorDatamanager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDatamanager.GetFilesData().Where(file => file.FilePath.Equals(oldFilePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (fileData != null)
            {
                if (this.IsFileInValidFolder(newFilePath, packageName))
                {
                    string templateName = string.Empty;

                    var extension = newFileName.Split('.').LastOrDefault();

                    var fileNameWithoutExtension = newFileName.Substring(0, newFileName.Length - (extension.Length + 1));

                    var viewFileExtensions = this.GetViewExtensions();

                    if (viewFileExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(packageName))
                        {
                            templateName = fileNameWithoutExtension;
                        }
                        else
                        {
                            templateName = packageName + "." + fileNameWithoutExtension;
                        }

                        fileData.FilePath = newFilePath;
                        fileData.FileName = newFileName;

                        fileMonitorDatamanager.SaveChanges();

                        this.CreateTemplate(templateName);
                    }
                }
            }
            else
            {
                this.FileAdded(newFileName, newFilePath, packageName);
            }
        }

        #endregion

        #region Private methods
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

            using (new Telerik.Sitefinity.Data.ElevatedModeRegion(pageManager))
            {
                if (pageManager.GetTemplates().Where(pt => pt.Title.Equals(templateTitle, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
                {
                    var template = pageManager.CreateTemplate();
                    // Set the custom template category id.
                    template.Category = Telerik.Sitefinity.Abstractions.SiteInitializer.CustomTemplatesCategoryId;
                    template.Name = templateTitle;
                    template.Title = templateTitle;

                    //the template must be using a Pure Mvc Mode
                    template.Framework = Pages.Model.PageTemplateFramework.Mvc;

                    //Publish the template in the invariant culture
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
