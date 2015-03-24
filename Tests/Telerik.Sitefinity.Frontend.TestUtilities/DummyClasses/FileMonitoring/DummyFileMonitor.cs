using System.IO;
using Telerik.Sitefinity.Frontend.FilesMonitoring;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.FileMonitoring
{
    /// <summary>
    /// This class represents a dummy implementation of <see cref="Telerik.Sitefinity.Frontend.FilesMonitoring.FileMonitor"/> functionality to make it testable.
    /// </summary>
    internal class DummyFileMonitor : FileMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyFileMonitor"/> class.
        /// </summary>
        public DummyFileMonitor()
        {
            this.ResourceFileManager = new DummyResourceFileManager();
        }

        #region Public properties 

        /// <summary>
        /// The resource file manager
        /// </summary>
        public DummyResourceFileManager ResourceFileManager { get; set; }

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

        #region Public methods

        /// <inheritdoc />
        public void FileChangedTest(string filePath, FileChangeType changeType, string oldFilePath = "")
        {
            this.FileChanged(filePath, changeType, oldFilePath);
        }

        #endregion 

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
            var path = Directory.GetCurrentDirectory() + virtualPath.Replace("~", string.Empty).Replace("/", "\\");

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
        protected override IFileManager GetResourceFileManager(string resourceFolder, string filePath)
        {
            return this.ResourceFileManager;
        }

        #endregion 
    }
}
