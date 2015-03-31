using System.Collections.Generic;
using Telerik.Sitefinity.Frontend.FilesMonitoring;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.FileMonitoring
{
    /// <summary>
    /// This class represents dummy implementation of <see cref="Telerik.Sitefinity.Frontend.FilesMonitoring.IFileManager" /> in order to test whether its methods are being invoked properly.
    /// </summary>
    internal class DummyResourceFileManager : IFileManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyResourceFileManager"/> class.
        /// </summary>
        public DummyResourceFileManager()
        {
            this.DummyFileInfos = new List<DummyFileInfo>();
        }

        /// <summary>
        /// The dummy file infos
        /// </summary>
        public ICollection<DummyFileInfo> DummyFileInfos { get; set; }

        /// <inheritdoc />
        public void FileAdded(string fileName, string filePath, string packageName = "")
        {
            this.DummyFileInfos.Add(new DummyFileInfo(fileName, filePath, FileChangeType.Created, packageName: packageName));
        }

        /// <inheritdoc />
        public void FileDeleted(string filePath, string packageName)
        {
            this.DummyFileInfos.Add(new DummyFileInfo(string.Empty, filePath, FileChangeType.Deleted, packageName: packageName));
        }

        /// <inheritdoc />
        public void FileRenamed(string newFileName, string oldFileName, string newFilePath, string oldFilePath, string packageName = "")
        {
            this.DummyFileInfos.Add(new DummyFileInfo(newFileName, newFilePath, FileChangeType.Renamed, oldFileName, oldFilePath, packageName));
        }
    }
}
