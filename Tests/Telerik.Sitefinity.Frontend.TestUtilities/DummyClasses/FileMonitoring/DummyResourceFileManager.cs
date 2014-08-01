using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.FilesMonitoring;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.FileMonitoring
{
    /// <summary>
    /// This class represents dummy implementation of <see cref="Telerik.Sitefinity.Frontend.FilesMonitoring.IFileManager" /> in order to test whether its methods are being invoked properly.
    /// </summary>
    public class DummyResourceFileManager : IFileManager
    {
        /// <inheritdoc />
        public void FileAdded(string fileName, string filePath, string packageName = "")
        {
            DummyFileInfos.Add(new DummyFileInfo(fileName, filePath, FileChangeType.Created, packageName: packageName));
        }

        /// <inheritdoc />
        public void FileDeleted(string filePath)
        {
            DummyFileInfos.Add(new DummyFileInfo("", filePath, FileChangeType.Deleted));
        }

        /// <inheritdoc />
        public void FileRenamed(string newFileName, string oldFileName, string newFilePath, string oldFilePath, string packageName = "")
        {
            DummyFileInfos.Add(new DummyFileInfo(newFileName, newFilePath, FileChangeType.Renamed, oldFileName, oldFilePath, packageName));
        }

        /// <summary>
        /// The dummy file infos
        /// </summary>
        public List<DummyFileInfo> DummyFileInfos = new List<DummyFileInfo>();
    }
}
