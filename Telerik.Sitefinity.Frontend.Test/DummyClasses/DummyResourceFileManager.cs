using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.FilesMonitoring;

namespace Telerik.Sitefinity.Frontend.Test.DummyClasses
{
    public class DummyResourceFileManager : IFileManager
    {
        public void FileAdded(string fileName, string filePath, string packageName = "")
        {
            DummyFileInfos.Add(new DummyFileInfo(fileName, filePath, FileChangeType.Created, packageName: packageName));
        }

        public void FileDeleted(string filePath)
        {
            DummyFileInfos.Add(new DummyFileInfo("", filePath, FileChangeType.Deleted));
        }

        public void FileRenamed(string newFileName, string oldFileName, string newFilePath, string oldFilePath, string packageName = "")
        {
            DummyFileInfos.Add(new DummyFileInfo(newFileName, newFilePath, FileChangeType.Renamed, oldFileName, oldFilePath, packageName));
        }

        public List<DummyFileInfo> DummyFileInfos = new List<DummyFileInfo>();
    }
}
