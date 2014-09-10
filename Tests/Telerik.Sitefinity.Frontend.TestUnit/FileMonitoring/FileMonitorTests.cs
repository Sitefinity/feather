using System.Collections.Generic;
using System.Linq;
using System.Web;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.FileMonitoring;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Project.Configuration;
using Telerik.Sitefinity.Security.Configuration;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.TestUnit.FileMonitoring
{
    /// <summary>
    /// Ensures that FileMonitor class works correctly.
    /// </summary>
    [TestClass]
    public class FileMonitorTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The file changed_ created_ invokes file manager.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether FileChanged method invoked with FileChangeTypes.Created will call FileAdded method of IFileManager with proper arguments.")]
        public void FileChanged_Created_InvokesFileManager()
        {
            // Arrange
            var fileMonitor = new DummyFileMonitor();
            fileMonitor.WatchedFoldersAndPackages.Add(new MonitoredDirectory("~/ResourcePackages/My package/Mvc/Views/Layouts", true));
            var filePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\\ResourcePackages\\My package\\Mvc\\Views\\Layouts\\test.cshtml", fileMonitor.AppPhysicalPath);
            var expectedFilePath = "~/ResourcePackages/My package/Mvc/Views/Layouts/test.cshtml";

            SystemManager.RunWithHttpContext(
                this.context, 
                () =>
                    {
                        // Act
                        fileMonitor.FileChangedTest(filePath, FileChangeType.Created);
                    });

            // Assert
            Assert.AreEqual(1, fileMonitor.ResourceFileManager.DummyFileInfos.Count(), "FileAdded method should be called.");
            Assert.AreEqual(FileChangeType.Created, fileMonitor.ResourceFileManager.DummyFileInfos.First().FileOperation, "FileAdded method is not called.");
            Assert.AreEqual("test.cshtml", fileMonitor.ResourceFileManager.DummyFileInfos.First().NewFileName, "FileAdded is called with wrong file name.");
            Assert.AreEqual(expectedFilePath, fileMonitor.ResourceFileManager.DummyFileInfos.First().NewFilePath, "FileAdded is called with wrong file path.");
            Assert.AreEqual("My package", fileMonitor.ResourceFileManager.DummyFileInfos.First().PackageName, "FileAdded is called with wrong package name.");
        }

        /// <summary>
        /// The file changed_ deleted_ invokes file manager.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether FileChanged method invoked with FileChangeTypes.Deleted will call FileDeleted method of IFileManager with proper arguments.")]
        public void FileChanged_Deleted_InvokesFileManager()
        {
            // Arrange
            var fileMonitor = new DummyFileMonitor();
            fileMonitor.WatchedFoldersAndPackages.Add(new MonitoredDirectory("~/ResourcePackages/My package/Mvc/Views/Layouts", true));
            var filePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\\ResourcePackages\\My package\\Mvc\\Views\\Layouts\\test.cshtml", fileMonitor.AppPhysicalPath);
            var expectedFilePath = "~/ResourcePackages/My package/Mvc/Views/Layouts/test.cshtml";

            SystemManager.RunWithHttpContext(
                this.context, 
                () =>
                    {
                        // Act
                        fileMonitor.FileChangedTest(filePath, FileChangeType.Deleted);
                    });

            // Assert
            Assert.AreEqual(1, fileMonitor.ResourceFileManager.DummyFileInfos.Count(), "FileDeleted method should be called.");
            Assert.AreEqual(FileChangeType.Deleted, fileMonitor.ResourceFileManager.DummyFileInfos.First().FileOperation, "FileDeleted method should be called.");
            Assert.AreEqual(expectedFilePath, fileMonitor.ResourceFileManager.DummyFileInfos.First().NewFilePath, "FileDeleted is called with wrong file name.");
        }

        /// <summary>
        /// The file changed_ renamed_ invokes file manager.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether FileChanged method invoked with FileChangeTypes.Renamed will call FileRenamed method of IFileManager with proper arguments.")]
        public void FileChanged_Renamed_InvokesFileManager()
        {
            // Arrange
            var fileMonitor = new DummyFileMonitor();
            fileMonitor.WatchedFoldersAndPackages.Add(new MonitoredDirectory("~/ResourcePackages/My package/Mvc/Views/Layouts", true));
            var oldFilePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\\ResourcePackages\\My package\\Mvc\\Views\\Layouts\\test.cshtml", fileMonitor.AppPhysicalPath);
            var expectedOldFilePath = "~/ResourcePackages/My package/Mvc/Views/Layouts/test.cshtml";

            var newFilePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\\ResourcePackages\\My package\\Mvc\\Views\\Layouts\\renamedTest.cshtml", fileMonitor.AppPhysicalPath);
            var expectedNewFilePath = "~/ResourcePackages/My package/Mvc/Views/Layouts/renamedTest.cshtml";

            SystemManager.RunWithHttpContext(
                this.context, 
                () =>
                    {
                        // Act
                        fileMonitor.FileChangedTest(newFilePath, FileChangeType.Renamed, oldFilePath);
                    });

            // Assert
            Assert.AreEqual(1, fileMonitor.ResourceFileManager.DummyFileInfos.Count(), "FileRenamed should be invoked.");
            Assert.AreEqual(FileChangeType.Renamed, fileMonitor.ResourceFileManager.DummyFileInfos.First().FileOperation, "FileRenamed should be invoked.");
            Assert.AreEqual("renamedTest.cshtml", fileMonitor.ResourceFileManager.DummyFileInfos.First().NewFileName, "FileRenamed is called with wrong file name.");
            Assert.AreEqual(expectedNewFilePath, fileMonitor.ResourceFileManager.DummyFileInfos.First().NewFilePath, "FileRenamed is called with wrong file path.");
            Assert.AreEqual("test.cshtml", fileMonitor.ResourceFileManager.DummyFileInfos.First().OldFileName, "FileRenamed is called with wrong old file name.");
            Assert.AreEqual(expectedOldFilePath, fileMonitor.ResourceFileManager.DummyFileInfos.First().OldFilePath, "FileRenamed is called with wrong old file path.");
            Assert.AreEqual("My package", fileMonitor.ResourceFileManager.DummyFileInfos.First().PackageName, "FileRenamed is called with wrong package name.");
        }

        /// <summary>
        /// The start_ non existing directories_ added in queued folders and packages.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Check whether Start method adds the non existing directories in QueuedFoldersAndPackages.")]
        public void Start_NonExistingDirectories_AddedInQueuedFoldersAndPackages()
        {
            // Arrange
            var packageFolderPath = "~/" + PackageManager.PackagesFolder;
            var baseMvcPath = "~/Mvc/Views/Layouts";
            var fileMonitor = new DummyFileMonitor();

            var directoriesInfo = new List<MonitoredDirectory>();
            directoriesInfo.Add(new MonitoredDirectory(packageFolderPath, true));
            directoriesInfo.Add(new MonitoredDirectory(baseMvcPath, false));

            // Act
            fileMonitor.Start(directoriesInfo);

            // Assert
            Assert.AreEqual(2, fileMonitor.QueuedFoldersAndPackages.Count(), "Both folders should be added in QueuedFoldersAndPackages");
            Assert.AreEqual(packageFolderPath, fileMonitor.QueuedFoldersAndPackages[0].Path, "The package folder path is not added correctly.");
            Assert.IsTrue(fileMonitor.QueuedFoldersAndPackages[0].IsPackage, "The values in QueuedFoldersAndPackages are not correct.");
            Assert.AreEqual(baseMvcPath, fileMonitor.QueuedFoldersAndPackages[1].Path, "The base Mvc folder path is not added correctly.");
            Assert.IsFalse(fileMonitor.QueuedFoldersAndPackages[1].IsPackage, "The values in QueuedFoldersAndPackages are not correct.");
        }

        /// <summary>
        /// The test cleanup.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.objectFactoryContainerRegion.Dispose();
            this.objectFactoryContainerRegion = null;
            this.context = null;
        }

        /// <summary>
        /// The test initialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.objectFactoryContainerRegion = new ObjectFactoryContainerRegion();
            ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
            ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
            Config.RegisterSection<ResourcesConfig>();
            Config.RegisterSection<SecurityConfig>();
            Config.RegisterSection<ProjectConfig>();

            this.context = new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null)));
        }

        #endregion

        #region Fields

        private HttpContextWrapper context;

        private ObjectFactoryContainerRegion objectFactoryContainerRegion;

        #endregion
    }
}