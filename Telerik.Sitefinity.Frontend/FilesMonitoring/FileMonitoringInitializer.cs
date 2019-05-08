using System;
using System.Collections.Generic;
using System.Configuration;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// This class contains logic for configuring the file monitoring functionality. 
    /// </summary>
    internal class FileMonitoringInitializer : IInitializer
    {
        /// <summary>
        /// Initializes and configure file monitoring functionality.
        /// </summary>
        public void Initialize()
        {
            if (!ObjectFactory.IsTypeRegistered(typeof(IFileMonitor)))
            {
                ObjectFactory.Container.RegisterType<IFileMonitor, FileMonitor>(new ContainerControlledLifetimeManager());
            }

            if (!ObjectFactory.IsTypeRegistered<IFileManager>(ResourceType.Layouts.ToString()))
            {
                ObjectFactory.Container.RegisterType<IFileManager, LayoutFileManager>(ResourceType.Layouts.ToString(), new ContainerControlledLifetimeManager());
            }

            if (!ObjectFactory.IsTypeRegistered<IFileManager>(ResourceType.Grid.ToString()))
            {
                ObjectFactory.Container.RegisterType<IFileManager, GridFileManager>(ResourceType.Grid.ToString(), new ContainerControlledLifetimeManager());
            }

            this.RegisterFileObservers();
        }

        /// <summary>
        /// Uninitializes the file monitoring functionality.
        /// </summary>
        public void Uninitialize()
        {
            if (this.fileMonitor != null)
            {
                this.fileMonitor.Dispose();
            }
        }

        /// <summary>
        /// Registers the file observers.
        /// </summary>
        private void RegisterFileObservers()
        {
            if (this.IsFileMonitoringEnabled())
            {
                this.fileMonitor = ObjectFactory.Resolve<IFileMonitor>();

                var monitoredDirectories = new List<MonitoredDirectory>();

                monitoredDirectories.Add(new MonitoredDirectory("~/" + PackageManager.PackagesFolder, true));
                monitoredDirectories.Add(new MonitoredDirectory("~/Mvc/Views/Layouts", false));
                monitoredDirectories.Add(new MonitoredDirectory("~/GridSystem/Templates", false));

                this.fileMonitor.Start(monitoredDirectories);
            }
        }

        private bool IsFileMonitoringEnabled()
        {
            // temporary solution to avoid any public APIs
            var enableWatcherSetting = ConfigurationManager.AppSettings["sf:featherFileSystemWatcherBehaviour"];
            if (!string.IsNullOrEmpty(enableWatcherSetting))
            {
                if (string.Equals(enableWatcherSetting, "true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if (string.Equals(enableWatcherSetting, "false", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            var sectionType = TypeResolutionService.ResolveType("Telerik.Sitefinity.SiteSync.Configuration.SiteSyncConfig", false);
            if (sectionType != null)
            {
                try
                {
                    var siteSyncConfig = Config.Get(sectionType);
                    var isTarget = (bool)siteSyncConfig["enabledAsTarget"];
                    if (isTarget)
                        return false;
                }
                catch (Exception)
                {
                    // the config is not available
                }
            }

            sectionType = TypeResolutionService.ResolveType("Telerik.Sitefinity.Packaging.Configuration.PackagingConfig", false);
            if (sectionType != null)
            {
                try
                {
                    var packagingConfig = Config.Get(sectionType);
                    var packagingMode = packagingConfig["packagingMode"];

                    var enumType = TypeResolutionService.ResolveType("Telerik.Sitefinity.Packaging.Restriction.PackagingMode", true);
                    var enumName = Enum.GetName(enumType, packagingMode);
                    if (enumName == "Target")
                        return false;
                }
                catch (Exception)
                {
                    // the config is not available
                }
            }

            return true;
        }

        private IFileMonitor fileMonitor;
    }
}
