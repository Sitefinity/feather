using Telerik.Sitefinity.Frontend.FilesMonitoring;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.FileMonitoring
{
    /// <summary>
    /// This class represents dummy DTO which contains file details.
    /// </summary>
    internal class DummyFileInfo
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyFileInfo"/> class.
        /// </summary>
        /// <param name="newFileName">New name of the file.</param>
        /// <param name="newFilePath">The new file path.</param>
        /// <param name="fileOperation">The file operation.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        /// <param name="oldFilePath">The old file path.</param>
        /// <param name="packageName">Name of the package.</param>
        public DummyFileInfo(string newFileName, string newFilePath, FileChangeType fileOperation, string oldFileName = "", string oldFilePath = "", string packageName = "")
        {
            this.NewFileName = newFileName;
            this.OldFileName = oldFileName;
            this.NewFilePath = newFilePath;
            this.OldFilePath = oldFilePath;
            this.PackageName = packageName;
            this.FileOperation = fileOperation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the file operation.
        /// </summary>
        /// <value>
        /// The file operation.
        /// </value>
        public FileChangeType FileOperation { get; set; }

        /// <summary>
        /// Gets or sets the new name of the file.
        /// </summary>
        /// <value>
        /// The new name of the file.
        /// </value>
        public string NewFileName { get; set; }

        /// <summary>
        /// Gets or sets the old name of the file.
        /// </summary>
        /// <value>
        /// The old name of the file.
        /// </value>
        public string OldFileName { get; set; }

        /// <summary>
        /// Gets or sets the old file path.
        /// </summary>
        /// <value>
        /// The old file path.
        /// </value>
        public string OldFilePath { get; set; }

        /// <summary>
        /// Gets or sets the new file path.
        /// </summary>
        /// <value>
        /// The new file path.
        /// </value>
        public string NewFilePath { get; set; }

        /// <summary>
        /// Gets or sets the name of the package.
        /// </summary>
        /// <value>
        /// The name of the package.
        /// </value>
        public string PackageName { get; set; }

        #endregion 
    }
}
