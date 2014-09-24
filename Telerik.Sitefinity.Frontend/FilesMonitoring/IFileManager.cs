namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// Classes that implement this interface should define the Sitefinity's behavior when a file is moved over the application folder structure.
    /// </summary>
    internal interface IFileManager
    {
        /// <summary>
        /// Reacts when a file is added to the folder.
        /// </summary>
        /// <param name="virtualFilePath">The virtual file path.</param>
        /// <param name="packageName">Name of the package.</param>
        void FileAdded(string fileName, string filePath, string packageName = "");

        /// <summary>
        /// Reacts on file deletion.
        /// </summary>
        /// <param name="path">The file path.</param>
        void FileDeleted(string filePath);

        /// <summary>
        /// Reacts on file renaming.
        /// </summary>
        /// <param name="newFileName">New name of the file.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        /// <param name="path">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        void FileRenamed(string newFileName, string oldFileName, string newFilePath, string oldFilePath, string packageName = "");
    }
}
