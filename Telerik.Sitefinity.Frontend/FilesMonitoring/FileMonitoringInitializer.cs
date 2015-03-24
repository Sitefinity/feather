using System.Collections.Generic;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// This class contains logic for configuring the file monitoring functionality. 
    /// </summary>
    internal class FileMonitoringInitializer
    {
        /// <summary>
        /// Initializes and configure file monitoring functionality.
        /// </summary>
        public void Initialize()
        {
            if (!ObjectFactory.Container.IsRegistered(typeof(IFileMonitor)))
            {
                ObjectFactory.Container.RegisterType<IFileMonitor, FileMonitor>(new ContainerControlledLifetimeManager());
            }

            if (!ObjectFactory.Container.IsRegistered(typeof(IFileManager), ResourceType.Layouts.ToString()))
            {
                ObjectFactory.Container.RegisterType<IFileManager, LayoutFileManager>(ResourceType.Layouts.ToString(), new ContainerControlledLifetimeManager());
            }

            if (!ObjectFactory.Container.IsRegistered(typeof(IFileManager), ResourceType.Grid.ToString()))
            {
                ObjectFactory.Container.RegisterType<IFileManager, GridFileManager>(ResourceType.Grid.ToString(), new ContainerControlledLifetimeManager());
            }

            this.RegisterFileObservers();
        }

        /// <summary>
        /// Registers the file observers.
        /// </summary>
        private void RegisterFileObservers()
        {
            var fileObserver = ObjectFactory.Resolve<IFileMonitor>();

            var monitoredDirectories = new List<MonitoredDirectory>();
            monitoredDirectories.Add(new MonitoredDirectory("~/" + PackageManager.PackagesFolder, true));
            monitoredDirectories.Add(new MonitoredDirectory("~/Mvc/Views/Layouts", false));

            fileObserver.Start(monitoredDirectories);
        }
    }
}
