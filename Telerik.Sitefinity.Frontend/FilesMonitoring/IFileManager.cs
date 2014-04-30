using System;
using System.Linq;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    public interface IFileManager
    {
        /// <summary>
        /// Process the file if such is added to the folder.
        /// </summary>
        /// <param name="virtualFilePath">The virtual file path.</param>
        /// <param name="packageName">Name of the package.</param>
        void FileAdded(string fileName, string filePath, string packageName = "");

        /// <summary>
        /// Called on file deletion
        /// </summary>
        /// <param name="path">The file path.</param>
        void FileDeleted(string filePath);

        /// <summary>
        /// Reacts on file renaming
        /// </summary>
        /// <param name="newFileName">New name of the file.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        /// <param name="path">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        void FileRenamed(string newFileName, string oldFileName, string newFilePath, string oldFilePath, string packageName = "");

    }
}
