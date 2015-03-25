using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    ///  This class manages the file monitoring and is responsible to fire events on changes in the observed file structure.
    /// </summary>
    internal class FileMonitor : IFileMonitor
    {
        #region IFileMonitor methods

        /// <summary>
        /// Observes the resources locations, watch for changes
        /// and take certain actions depending on the change
        /// </summary>
        /// <param name="directoriesInfo">The monitored directories.</param>
        public void Start(IList<MonitoredDirectory> directoriesInfo)
        {
            if (this.rootWatcher == null)
            {
                this.AddRootWatcher();
            }

            foreach (var directory in directoriesInfo)
            {
                var direcotryPath = this.MapPath(directory.Path);

                if (this.WatchedFoldersAndPackages.Contains(directory))
                    continue;

                DirectoryInfo dir = new DirectoryInfo(direcotryPath);

                if (!dir.Exists)
                {
                    this.QueuedFoldersAndPackages.Add(directory);
                    continue;
                }
                else
                {
                    this.WatchedFoldersAndPackages.Add(directory);
                    this.ProcessDirecotryFiles(dir);
                    this.AddFileWatcher(directory);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.fileWatchers != null)
            {
                foreach (var watcher in this.fileWatchers)
                {
                    if (watcher.Value != null)
                    {
                        watcher.Value.Dispose();
                    }
                }
            }

            if (this.rootWatcher != null)
                this.rootWatcher.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Takes appropriate actions based on the resource file type
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="oldFilePath">The old file path.</param>
        protected virtual void FileChanged(string filePath, FileChangeType changeType, string oldFilePath = "")
        {
            var virtualFilePath = this.ConvertToVirtualPath(filePath);
            var oldFileVirtualPath = this.ConvertToVirtualPath(oldFilePath);

            var watchedDirInfo = this.WatchedFoldersAndPackages.FirstOrDefault(dirInfo => virtualFilePath.StartsWith(dirInfo.Path, StringComparison.Ordinal));

            if (watchedDirInfo == null)
                return;

            string packageName = string.Empty;

            var resourceDirecotryPath = VirtualPathUtility.GetDirectory(virtualFilePath);
            var resourceDirectoryTree = resourceDirecotryPath.Split('/');

            if (resourceDirectoryTree.Length < 3)
                return;

            if (watchedDirInfo.IsPackage)
                packageName = resourceDirectoryTree[2];

            string resourceFolder = resourceDirectoryTree[resourceDirectoryTree.Length - 2];

            var fileName = virtualFilePath.Replace(resourceDirecotryPath, string.Empty);

            SystemManager.RunWithElevatedPrivilegeDelegate elevDelegate = this.GetFileChangedDelegate(new FileChangedDelegateArguments()
            {
                FilePath = virtualFilePath,
                ChangeType = changeType,
                OldFilePath = oldFileVirtualPath, 
                PackageName = packageName,
                ResourceFolder = resourceFolder,
                FileName = fileName
            });
            SystemManager.RunWithElevatedPrivilege(elevDelegate);
        }

        /// <summary>
        /// Removes the non existing files data.
        /// </summary>
        protected virtual void RemoveNonExistingData()
        {
            SystemManager.RunWithElevatedPrivilegeDelegate elevDelegate = (p) =>
            {
                var fileMonitorDataManager = FileMonitorDataManager.GetManager();

                // finds all records containing file paths which no longer exists
                var nonExistingFilesData = fileMonitorDataManager.GetFilesData().Select(fd => fd.FilePath).ToArray()
                    .Where(f => !HostingEnvironment.VirtualPathProvider.FileExists(f));

                if (nonExistingFilesData != null && nonExistingFilesData.Any())
                {
                    // remove all records in the lists
                    foreach (var filePath in nonExistingFilesData)
                    {
                        var resourceDirectoryTree = VirtualPathUtility.GetDirectory(filePath).Split('/');

                        if (resourceDirectoryTree.Length >= 2)
                        {
                            var packageFolderIdx = Array.IndexOf(resourceDirectoryTree, PackageManager.PackagesFolder);
                            string packageName = packageFolderIdx > 0 && packageFolderIdx + 1 < resourceDirectoryTree.Length ? resourceDirectoryTree[packageFolderIdx + 1] : string.Empty;
                            string resourceFolder = resourceDirectoryTree[resourceDirectoryTree.Length - 2];
                            IFileManager resourceFilesManager = this.GetResourceFileManager(resourceFolder, filePath);
                            resourceFilesManager.FileDeleted(filePath, packageName);
                        }
                    }
                }
            };

            SystemManager.RunWithElevatedPrivilege(elevDelegate);
        }

        /// <summary>
        /// Gets the resource file manager.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>FileManager</returns>
        protected virtual IFileManager GetResourceFileManager(string resourceFolder, string resourcePath)
        {
            IFileManager resourceFilesManager = null;
            var resourceType = this.GetResourceType(resourceFolder, resourcePath);

            // the resource folder must follow the convention and the folder name must corresponds to a resource type
            if (resourceType != null)
            {
                if (ObjectFactory.IsTypeRegistered<IFileManager>(resourceType.ToString()))
                    resourceFilesManager = ObjectFactory.Resolve<IFileManager>(resourceType.ToString());
            }

            return resourceFilesManager;
        }

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        /// <param name="resourceFolder">The resource folder.</param>
        /// <returns></returns>
        protected virtual ResourceType? GetResourceType(string resourceFolder, string resourcePath)
        {
            if (resourcePath.Contains("/GridSystem/Templates/"))
                return ResourceType.Grid;

            ResourceType resourceType;
            if (Enum.TryParse<ResourceType>(resourceFolder, true, out resourceType))
                return resourceType;
            else
                return null;
        }

        /// <summary>
        /// Gets the physical path of the application.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetApplicationPhysicalPath()
        {
            return HostingEnvironment.ApplicationPhysicalPath;
        }

        /// <summary>
        /// Maps the virtual path to physical path.
        /// </summary>
        /// <param name="virtualPath">The virtual Path.</param>
        /// <returns>The physical path on the server specified by virtualPath.</returns>
        protected virtual string MapPath(string virtualPath)
        {
            return FrontendManager.VirtualPathBuilder.MapPath(virtualPath);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Adds the root watcher.
        /// </summary>
        private void AddRootWatcher()
        {
            this.rootWatcher = new FileSystemWatcher();

            this.rootWatcher.Created += this.OnRootCreated;
            this.rootWatcher.Deleted += this.OnRootDeleted;
            this.rootWatcher.Renamed += this.OnRootRenamed;

            this.rootWatcher.IncludeSubdirectories = true;
            this.rootWatcher.Path = this.GetApplicationPhysicalPath();
            this.rootWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            this.rootWatcher.EnableRaisingEvents = true;

            this.RemoveNonExistingData();
        }

        /// <summary>
        /// Adds the file watcher.
        /// </summary>
        /// <param name="directory">The directory.</param>
        private void AddFileWatcher(MonitoredDirectory directory)
        {
            var fileWatcher = new FileSystemWatcher();

            fileWatcher.IncludeSubdirectories = directory.IsPackage;
            fileWatcher.Path = this.MapPath(directory.Path);
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

            fileWatcher.Created += this.OnFileCreated;
            fileWatcher.Deleted += this.OnFileDeleted;
            fileWatcher.Renamed += this.OnFileRenamed;

            fileWatcher.EnableRaisingEvents = true;

            this.FileWatchers.Add(directory.Path, fileWatcher);
        }

        /// <summary>
        /// Removes the file watcher.
        /// </summary>
        /// <param name="queuedDirInfoKey">The queued dir information key.</param>
        private void RemoveFileWatcher(string queuedDirInfoKey)
        {
            if (this.FileWatchers.ContainsKey(queuedDirInfoKey))
            {
                var watcher = this.FileWatchers.Where(f => f.Key.Equals(queuedDirInfoKey)).First();

                watcher.Value.EnableRaisingEvents = false;

                this.FileWatchers.Remove(queuedDirInfoKey);

                watcher.Value.Created -= new FileSystemEventHandler(this.OnFileCreated);
                watcher.Value.Deleted -= new FileSystemEventHandler(this.OnFileDeleted);
                watcher.Value.Renamed -= new RenamedEventHandler(this.OnFileRenamed);

                watcher.Value.Dispose();
            }
        }

        /// <summary>
        /// Converts to virtual path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string ConvertToVirtualPath(string path)
        {
            var appPhysicalPath = this.GetApplicationPhysicalPath();

            // converting the file path to a virtual file path
            if (!appPhysicalPath.IsNullOrEmpty() && !path.IsNullOrEmpty())
                path = path.Substring(appPhysicalPath.Length - 1);

            var virtualFilePath = path.Replace('\\', '/').Insert(0, "~");

            return virtualFilePath;
        }

        /// <summary>
        /// Processes all files under a directory, including subfolders.
        /// </summary>
        /// <param name="dirInfo">The directory information.</param>
        private void ProcessDirecotryFiles(DirectoryInfo dirInfo)
        {
            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var filePath = file.FullName;
                    this.FileChanged(filePath, FileChangeType.Created);
                }
                catch (IOException ex)
                {
                    Log.Write(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Exception occurred while processing the file named {0}, details: {1}", file.Name, ex));
                }
            }
        }

        /// <summary>
        /// Called when root directory is renamed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void OnRootRenamed(object sender, RenamedEventArgs e)
        {
            string directoryPath = e.FullPath;
            string oldDirecotoryPath = e.OldFullPath;

            this.QueueWatch(oldDirecotoryPath);

            this.StartWatch(directoryPath);
        }

        /// <summary>
        /// Called when root directory is created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnRootCreated(object sender, FileSystemEventArgs e)
        {
            this.StartWatch(e.FullPath);
        }

        /// <summary>
        /// Called when root directory is deleted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnRootDeleted(object sender, FileSystemEventArgs e)
        {
            this.QueueWatch(e.FullPath);
        }

        /// <summary>
        /// Called when a file is renamed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void OnFileRenamed(object source, RenamedEventArgs e)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
            {
                this.RemoveNonExistingData();

                DirectoryInfo dirInfo = new DirectoryInfo(e.FullPath);

                if (dirInfo.Exists)
                    this.ProcessDirecotryFiles(dirInfo);
            }
            else
            {
                this.FileChanged(e.FullPath, FileChangeType.Renamed, e.OldFullPath);
            }
        }

        /// <summary>
        /// Called when a file is created.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnFileCreated(object source, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
                return;

            this.FileChanged(e.FullPath, FileChangeType.Created);
        }

        /// <summary>
        /// Called when a file is deleted.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnFileDeleted(object source, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
                this.RemoveNonExistingData();
            else
                this.FileChanged(e.FullPath, FileChangeType.Deleted);
        }

        /// <summary>
        /// Queues the watch action for certain directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        private void QueueWatch(string directoryPath)
        {
            var virtualFilePath = this.ConvertToVirtualPath(directoryPath);

            var queuedDirInfo = this.WatchedFoldersAndPackages.FirstOrDefault(dirInfo => dirInfo.Path.StartsWith(virtualFilePath, StringComparison.OrdinalIgnoreCase));

            if (queuedDirInfo == null)
                return;           

            this.QueuedFoldersAndPackages.Add(queuedDirInfo);

            this.WatchedFoldersAndPackages.Remove(queuedDirInfo);

            this.RemoveFileWatcher(queuedDirInfo.Path);

            var directories = new List<MonitoredDirectory>();
            this.Start(directories);
        }

        /// <summary>
        /// Starts the watch action for certain directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        private void StartWatch(string directoryPath)
        {
            var virtualFilePath = this.ConvertToVirtualPath(directoryPath);

            var queuedDirInfo = this.QueuedFoldersAndPackages.FirstOrDefault(dirInfo => dirInfo.Path.StartsWith(virtualFilePath, StringComparison.OrdinalIgnoreCase));

            if (queuedDirInfo == null)
                return;

            this.QueuedFoldersAndPackages.Remove(queuedDirInfo);

            var directories = new List<MonitoredDirectory>();
            directories.Add(queuedDirInfo);
            this.Start(directories);
        }

        /// <summary>
        /// Gets the file changed delegate. It will call the appropriate methods of <see cref="IFileManager"/> when the file structure has changed. 
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>RunWithElevatedPrivilegeDelegate</returns>
        private SystemManager.RunWithElevatedPrivilegeDelegate GetFileChangedDelegate(FileChangedDelegateArguments args)
        {
            return (p) =>
            {
                // get the resource file manager depending on its type
                IFileManager resourceFilesManager = this.GetResourceFileManager(args.ResourceFolder, args.FilePath);

                if (resourceFilesManager != null)
                {
                    switch (args.ChangeType)
                    {
                        case FileChangeType.Created:
                            {
                                resourceFilesManager.FileAdded(args.FileName, args.FilePath, args.PackageName);
                                break;
                            }

                        case FileChangeType.Deleted:
                            {
                                resourceFilesManager.FileDeleted(args.FilePath, args.PackageName);
                                break;
                            }

                        case FileChangeType.Renamed:
                            {
                                var fileNameStartIndex = args.OldFilePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1;
                                var oldFileName = args.OldFilePath.Substring(fileNameStartIndex);
                                resourceFilesManager.FileRenamed(args.FileName, oldFileName, args.FilePath, args.OldFilePath, args.PackageName);
                                break;
                            }
                    }
                }
            };
        }

        #endregion

        #region Private fields

        private Dictionary<string, FileSystemWatcher> fileWatchers;
        private FileSystemWatcher rootWatcher = null;
        private IList<MonitoredDirectory> queuedFoldersAndPackages;
        private IList<MonitoredDirectory> watchedFoldersAndPackages;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the watched folders and packages.
        /// </summary>
        internal IList<MonitoredDirectory> WatchedFoldersAndPackages
        {
            get
            {
                if (this.watchedFoldersAndPackages == null)
                    this.watchedFoldersAndPackages = new List<MonitoredDirectory>();

                return this.watchedFoldersAndPackages;
            }
        }

        /// <summary>
        /// Gets the queued folders and packages.
        /// </summary>
        internal IList<MonitoredDirectory> QueuedFoldersAndPackages
        {
            get
            {
                if (this.queuedFoldersAndPackages == null)
                    this.queuedFoldersAndPackages = new List<MonitoredDirectory>();

                return this.queuedFoldersAndPackages;
            }
        }

        /// <summary>
        /// Gets the file watchers.
        /// </summary>
        /// <value>
        /// The file watchers.
        /// </value>
        internal IDictionary<string, FileSystemWatcher> FileWatchers
        {
            get
            {
                if (this.fileWatchers == null)
                    this.fileWatchers = new Dictionary<string, FileSystemWatcher>();

                return this.fileWatchers;
            }
        }

        #endregion

        #region Private classes

        /// <summary>
        /// This class represents the arguments for invoking the  GetFileChangedDelegate method.
        /// </summary>
        private class FileChangedDelegateArguments
        {
            public string FilePath { get; set; }

            public FileChangeType ChangeType { get; set; }

            public string OldFilePath { get; set; }

            public string PackageName { get; set; }

            public string ResourceFolder { get; set; }

            public string FileName { get; set; }
        }

        #endregion
    }
}
