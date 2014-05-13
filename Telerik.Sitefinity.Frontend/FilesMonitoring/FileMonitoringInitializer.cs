using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// This class contains logic for configuring the file monitoring functionality. 
    /// </summary>
    public class FileMonitoringInitializer
    {
        /// <summary>
        /// Initializes and configure file monitoring functionality.
        /// </summary>
        public void Initialize()
        {
            ObjectFactory.Container.RegisterType<IFileMonitor, FileMonitor>(new ContainerControlledLifetimeManager());
            ObjectFactory.Container.RegisterType<IFileManager, LayoutFileManager>(ResourceType.Layouts.ToString(), new ContainerControlledLifetimeManager());

            this.RegisterFileObservers();
        }

        /// <summary>
        /// Registers the file observers.
        /// </summary>
        private void RegisterFileObservers()
        {
            var fileObserver = ObjectFactory.Resolve<IFileMonitor>();

            var monitoredDirectories = new List<MonitoredDirectory>();
            monitoredDirectories.Add(new MonitoredDirectory("~/" + PackagesManager.PackagesFolder, true));
            monitoredDirectories.Add(new MonitoredDirectory("~/Mvc/Views/Layouts", false));

            fileObserver.Start(monitoredDirectories);
        }
    }
}
