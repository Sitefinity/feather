using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// Represents the file operation type 
    /// </summary>
    public enum FileChangeTypes
    {
        Created,
        Deleted,
        Renamed
    }

    public class FileMonitor : IFilesMonitor
    {
        #region Properties

        /// <summary>
        /// Gets the watched folders and packages.
        /// </summary>
        /// <value>
        /// Dictionary with all watched folders and packages. The bool param shows if the folder is package
        /// </value>
        internal Dictionary<string, bool> WatchedFoldersAndPackages
        {
            get
            {
                if (this.watchedFoldersAndPackages == null)
                {
                    this.watchedFoldersAndPackages = new Dictionary<string, bool>();
                }
                return this.watchedFoldersAndPackages;
            }
        }

        /// <summary>
        /// Gets the queued folders and packages.
        /// </summary>
        /// <value>
        /// Dictionary with all queued folders and packages. The bool param shows if the folder is package
        /// </value>
        internal Dictionary<string, bool> QueuedFoldersAndPackages
        {
            get
            {
                if (this.queuedFoldersAndPackages == null)
                {
                    this.queuedFoldersAndPackages = new Dictionary<string, bool>();
                }
                return this.queuedFoldersAndPackages;
            }
        }

        /// <summary>
        /// Gets the file watchers.
        /// </summary>
        /// <value>
        /// The file watchers.
        /// </value>
        internal Dictionary<string, FileSystemWatcher> FileWatchers
        {
            get
            {
                if (this.fileWatchers == null)
                {
                    this.fileWatchers = new Dictionary<string, FileSystemWatcher>();
                }
                return this.fileWatchers;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Observes the resources locations, watch for changes
        /// and take certain actions depending on the change
        /// </summary>
        /// <param name="directoriesInfo">The directories to be watched.</param>
        public void Start(Dictionary<string, bool> directoriesInfo)
        {
            //if the root watcher does not exist it must be created
            if (rootWatcher == null)
            {
                this.rootWatcher = new FileSystemWatcher();

                //register the events
                this.rootWatcher.Created += OnRootChanged;
                this.rootWatcher.Deleted += OnRootChanged;
                this.rootWatcher.Renamed += OnRootRenamed;

                //all sub folders are included
                this.rootWatcher.IncludeSubdirectories = true;

                //Set the directory path
                this.rootWatcher.Path = HostingEnvironment.ApplicationPhysicalPath;

                //notify only when a direcotry is changed
                this.rootWatcher.NotifyFilter = NotifyFilters.DirectoryName;

                //start the watcher
                this.rootWatcher.EnableRaisingEvents = true;

                //Remove records for files that no longer exists
                this.RemoveNonExistingData();
            }


            foreach (var directory in directoriesInfo)
            {
                var direcotryPath = HostingEnvironment.MapPath(directory.Key);

                //the directory is already being watched for changes
                if (this.WatchedFoldersAndPackages.ContainsKey(directory.Key))
                    continue;

                DirectoryInfo dir = new DirectoryInfo(direcotryPath);

                if (!dir.Exists)
                {
                    //the directory does not exist so it is added for a queue list and will be regularly checked for existence
                    this.QueuedFoldersAndPackages.Add(directory.Key, directory.Value);
                    continue;
                }
                else
                {
                    //adds the directory information in the list directories that are being watched
                    this.WatchedFoldersAndPackages.Add(directory.Key, directory.Value);

                    //process the existing inside this directory files 
                    this.ProcessDirecotry(dir);
                }

                var fileWatcher = new FileSystemWatcher();

                //watch inside sub directories only if its a package root directory
                fileWatcher.IncludeSubdirectories = directory.Value;

                //Set the directory path
                fileWatcher.Path = direcotryPath;

                /* Watch for changes in the renaming of files or directories. */
                fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

                // Add event handlers.
                fileWatcher.Created += new FileSystemEventHandler(this.OnFileChanged);
                fileWatcher.Deleted += new FileSystemEventHandler(this.OnFileChanged);
                fileWatcher.Renamed += new RenamedEventHandler(this.OnFileRenamed);

                // Begin watching.
                fileWatcher.EnableRaisingEvents = true;

                //Add the watcher to the dictionary
                this.FileWatchers.Add(directory.Key, fileWatcher);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="path">The file path.</param>
        public void Stop(string filePath)
        {

        }

        #endregion

        #region Protected nethods

        /// <summary>
        /// Takes appropriate actions based on the resource file type
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="oldFilePath">The old file path.</param>
        protected virtual void FileChanged(string filePath, Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes changeType, string oldFilePath = "")
        {
            var virtualFilePath = this.ConvertToVirtualPath(filePath);

            string packageName = string.Empty;

            var fileName = VirtualPathUtility.GetFileName(virtualFilePath);

            //get the directory tree part from the virtal path
            var resourceDirectoryTree = VirtualPathUtility.GetDirectory(virtualFilePath).Split('/');

            //the resource folder is actualy the file parent folder
            string resourceFolder = resourceDirectoryTree[resourceDirectoryTree.Length - 2];

            string oldFileName = string.Empty;

            if (changeType == FileChangeTypes.Renamed)
            {
                var oldVirtualFilePath = filePath.Substring(HostingEnvironment.ApplicationPhysicalPath.Length - 1).Replace('\\', '/');
                oldFileName = VirtualPathUtility.GetFileName(oldVirtualFilePath);
            }

            //get the watched directory info from the list
            var watchedDirInfos = this.WatchedFoldersAndPackages.Where(dirInfo => virtualFilePath.StartsWith(dirInfo.Key, StringComparison.InvariantCultureIgnoreCase));

            //continue only if the directory is in the list
            if (watchedDirInfos.Count() > 0)
            {
                var watchedDirInfo = watchedDirInfos.First();

                //if the watched directory information shows that this a package directory
                //extract the package name out of the virtual path
                if (watchedDirInfo.Value)
                {
                    packageName = resourceDirectoryTree[2];
                }
                SystemManager.RunWithElevatedPrivilegeDelegate elevDelegate = (p) =>
                {
                    //get the resource file manager depending on its type
                    IFileManager resourceFilesManager = this.GetResourceFileManager(resourceFolder);

                    if (resourceFilesManager != null)
                    {
                        switch (changeType)
                        {
                            case Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Created:
                                resourceFilesManager.FileAdded(fileName, filePath, packageName);
                                break;
                            case Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Deleted:
                                resourceFilesManager.FileDeleted(filePath);
                                break;
                            case Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Renamed:
                                resourceFilesManager.FileRenamed(fileName, oldFileName, filePath, oldFilePath, packageName);
                                break;
                        }
                    }
                };

                SystemManager.RunWithElevatedPrivilege(elevDelegate);

            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the resource file manager.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns></returns>
        protected virtual IFileManager GetResourceFileManager(string resourceFolder)
        {
            IFileManager resourceFilesManager = null;

            ResourceTypes resourceType;

            //the resource folder must follow the convention and the folder name must corresponds to a resource type
            if (Enum.TryParse<ResourceTypes>(resourceFolder, true, out resourceType))
            {
                if (ObjectFactory.IsTypeRegistered<IFileManager>(resourceType.ToString()))
                    resourceFilesManager = ObjectFactory.Resolve<IFileManager>(resourceType.ToString());
            }

            return resourceFilesManager;
        }

        /// <summary>
        /// Removes the non existing files data.
        /// </summary>
        private void RemoveNonExistingData()
        {
            SystemManager.RunWithElevatedPrivilegeDelegate elevDelegate = (p) =>
            {
                var fileMonitorDataManager = FileMonitorDataManager.GetManager();

                //finds all records containing file paths which no longer exists
                var nonExistingFilesData = fileMonitorDataManager.GetFilesData().Select(fd => fd.FilePath).ToArray()
                    .Where(f => !File.Exists(f));

                if (nonExistingFilesData != null && nonExistingFilesData.Any())
                {
                    //remove all records in the lists
                    foreach (var filePath in nonExistingFilesData)
                    {
                        //converting the file path to a virtual file path
                        var virtualFilePath = this.ConvertToVirtualPath(filePath);

                        //get the directory tree part from the virtal path
                        var resourceDirectoryTree = VirtualPathUtility.GetDirectory(virtualFilePath).Split('/');

                        //the resource folder is actualy the file parent folder
                        string resourceFolder = resourceDirectoryTree[resourceDirectoryTree.Length - 2];

                        //get the resource file manager depending on its type
                        IFileManager resourceFilesManager = this.GetResourceFileManager(resourceFolder);

                        resourceFilesManager.FileDeleted(filePath);
                    }
                }
            };

            SystemManager.RunWithElevatedPrivilege(elevDelegate);
        }

        /// <summary>
        /// Converts to virtual path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string ConvertToVirtualPath(string path)
        {
            //converting the file path to a virtual file path
            var virtualFilePath = path.Substring(HostingEnvironment.ApplicationPhysicalPath.Length - 1).Replace('\\', '/').Insert(0, "~");
            return virtualFilePath;
        }

        /// <summary>
        /// Precesses the direcotry.
        /// </summary>
        /// <param name="dirInfo">The directroty information.</param>
        private void ProcessDirecotry(DirectoryInfo dirInfo)
        {
            //get all files including files in sub directories
            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var filePath = file.FullName;
                //process the file
                this.FileChanged(filePath, Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Created);
            }
        }

        /// <summary>
        /// Called when [root renamed].
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
        /// Called when [root changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnRootChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                string directoryPath = e.FullPath;

                this.StartWatch(directoryPath);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                this.QueueWatch(e.FullPath);
            }

        }

        /// <summary>
        /// Called when [file renamed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void OnFileRenamed(object source, RenamedEventArgs e)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
            {
                //Remove records for files that no longer exists
                this.RemoveNonExistingData();

                DirectoryInfo dirInfo = new DirectoryInfo(e.FullPath);
                if (dirInfo.Exists)
                {
                    this.ProcessDirecotry(dirInfo);
                }
            }
            else
            {
                this.FileChanged(e.FullPath, Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Renamed, e.OldFullPath);
            }
        }

        /// <summary>
        /// Called when [file changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
            {
                if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    //Remove records for files that no longer exists
                    this.RemoveNonExistingData();
                }
            }
            else
            {
                Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes changeType = Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Created;
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    changeType = Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Created;
                }
                else if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    changeType = Telerik.Sitefinity.Frontend.FilesMonitoring.FileChangeTypes.Deleted;
                }
                else
                {
                    return;
                }
                this.FileChanged(e.FullPath, changeType);
            }
        }

        /// <summary>
        /// Queues the watch action for certain directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        private void QueueWatch(string directoryPath)
        {
            //convert the file path to a virtual file path
            var virtualFilePath = this.ConvertToVirtualPath(directoryPath);

            //check if the directory is being watched
            var watchedDirInfos = this.WatchedFoldersAndPackages.Where(dirInfo => dirInfo.Key.StartsWith(virtualFilePath, StringComparison.InvariantCultureIgnoreCase));// as Dictionary<string, bool>;

            if (watchedDirInfos.Count() > 0)
            {
                var queuedDirInfo = watchedDirInfos.First();

                var dictionary = new Dictionary<string, bool>();

                //adding the directory to the queue of direcotries which must be regularly ckecked for existence
                this.QueuedFoldersAndPackages.Add(queuedDirInfo.Key, queuedDirInfo.Value);

                //removing the direcotry from the list of watched directories
                this.WatchedFoldersAndPackages.Remove(queuedDirInfo.Key);

                //disposing the file watcher responsible for this directory
                if (this.FileWatchers.ContainsKey(queuedDirInfo.Key))
                {
                    var watcher = this.FileWatchers.Where(f => f.Key.Equals(queuedDirInfo.Key)).First();

                    watcher.Value.EnableRaisingEvents = false;

                    this.FileWatchers.Remove(queuedDirInfo.Key);

                    watcher.Value.Created -= new FileSystemEventHandler(this.OnFileChanged);
                    watcher.Value.Deleted -= new FileSystemEventHandler(this.OnFileChanged);
                    watcher.Value.Renamed -= new RenamedEventHandler(this.OnFileRenamed);

                    watcher.Value.Dispose();
                }

                this.Start(dictionary);
            }
        }

        /// <summary>
        /// Starts the watch action for certain directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        private void StartWatch(string directoryPath)
        {
            //convert the file path to a virtual file path
            var virtualFilePath = this.ConvertToVirtualPath(directoryPath);

            //check if the directory is in the queue 
            var queuedDirInfos = this.QueuedFoldersAndPackages.Where(dirInfo => dirInfo.Key.StartsWith(virtualFilePath, StringComparison.InvariantCultureIgnoreCase));// as Dictionary<string, bool>;

            if (queuedDirInfos.Count() > 0)
            {
                var queuedDirInfo = queuedDirInfos.First();

                var dictionary = new Dictionary<string, bool>();

                dictionary.Add(queuedDirInfo.Key, queuedDirInfo.Value);

                //removing the direcotry from the queue
                this.QueuedFoldersAndPackages.Remove(queuedDirInfo.Key);

                //process and start watching the directory
                this.Start(dictionary);
            }
        }

        #endregion

        #region Private fields

        private Dictionary<string, FileSystemWatcher> fileWatchers;
        private FileSystemWatcher rootWatcher = null;
        private Dictionary<string, bool> queuedFoldersAndPackages;
        private Dictionary<string, bool> watchedFoldersAndPackages;

        #endregion

    }
}
