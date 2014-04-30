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
        public void Initialize()
        {
            ObjectFactory.Container.RegisterType<IFilesMonitor, FileMonitor>(new ContainerControlledLifetimeManager());
            ObjectFactory.Container.RegisterType<IFileManager, LayoutFilesManager>(ResourceTypes.Layouts.ToString(), new ContainerControlledLifetimeManager());

            this.RegisterFileObservers();
        }

        /// <summary>
        /// Registers the file observers.
        /// </summary>
        private void RegisterFileObservers()
        {
            var fileObserver = ObjectFactory.Resolve<IFilesMonitor>();

            var direcotoriesInfo = new Dictionary<string, bool>();
            direcotoriesInfo.Add("~/" + PackagesManager.PackagesFolder, true);
            direcotoriesInfo.Add("~/Mvc/Views/Layouts", false);

            fileObserver.Start(direcotoriesInfo);
        }
    }
}
