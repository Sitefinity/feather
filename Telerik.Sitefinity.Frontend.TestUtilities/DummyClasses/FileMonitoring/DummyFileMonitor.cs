﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.FilesMonitoring;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.FileMonitoring
{
    /// <summary>
    /// This class represents a dummy implementation of <see cref="Telerik.Sitefinity.Frontend.FilesMonitoring.FileMonitor"/> functionality to make it testable.
    /// </summary>
    public class DummyFileMonitor : FileMonitor
    {
        #region Public properties 

        /// <summary>
        /// Gets the application physical path.
        /// </summary>
        /// <value>
        /// The application physical path.
        /// </value>
        public string AppPhysicalPath
        {
            get
            {
                var currentDir = Directory.GetCurrentDirectory();
                var fileInfo = Directory.GetParent(currentDir).Parent as FileSystemInfo;

                if (fileInfo == null)
                    return currentDir;
                else
                    return fileInfo.FullName;
            }
        }

        #endregion 

        #region Overrides 

        /// <summary>
        /// Gets the application physical path.
        /// </summary>
        /// <returns></returns>
        protected override string GetApplicationPhysicalPath()
        {
            return this.AppPhysicalPath + "\\";
        }

        /// <summary>
        /// Maps the virtual path to physical path.
        /// </summary>
        /// <param name="virtualPath">The virtual Path.</param>
        /// <returns></returns>
        protected override string MapPath(string virtualPath)
        {
            var path =  Directory.GetCurrentDirectory() + virtualPath.Replace("~","").Replace("/", "\\");

            return path;
        }

        /// <summary>
        /// Dummies removing of the non existing files data.
        /// </summary>
        protected override void RemoveNonExistingData()
        {
        }

        /// <summary>
        /// Gets dummy resource manager.
        /// </summary>
        /// <param name="resourceFolder"></param>
        /// <returns></returns>
        protected override IFileManager GetResourceFileManager(string resourceFolder)
        {
            return this.ResourceFileManager;
        }

        #endregion 

        #region Public methods

        /// <inheritdoc />
        public void FileChangedTest(string filePath, FileChangeType changeType, string oldFilePath = "") 
        {
            this.FileChanged(filePath, changeType, oldFilePath);
        }

        #endregion 

        #region Fields and constants

        /// <summary>
        /// The resource file manager
        /// </summary>
        public DummyResourceFileManager ResourceFileManager = new DummyResourceFileManager();

        #endregion 
    }
}
