using System;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.Designers;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// A service that will be invoked by Sitefinity.
    /// </summary>
    public class FrontendService : ServiceBase
    {
        /// <summary>
        /// Gets the types of the service used to register and resolve the service from the Service Bus.
        /// </summary>
        /// <value>The type of the service.</value>
        public override Type[] Interfaces
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes the service with specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;
            Bootstrapper.Initialized += this.Bootstrapper_Initialized;
        }

        /// <summary>
        /// Handles the Initialized event of the Bootstrapper.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sitefinity.Data.ExecutedEventArgs"/> instance containing the event data.</param>
        protected virtual void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                var resourcesInitializer = new ResourcesInitiliazer();
                resourcesInitializer.Initialize();

                var fileMonitoringInitilizer = new FileMonitoringInitializer();
                fileMonitoringInitilizer.Initialize();

                var controllerContainerInitializer = new ControllerContainerInitializer();
                controllerContainerInitializer.Initialize();

                var layoutsInitializer = new LayoutInitializer();
                layoutsInitializer.Initialize();

                var gridSystemInitializer = new GridSystemInitializer();
                gridSystemInitializer.Initialize();

                var designerInitializer = new DesignerInitializer();
                designerInitializer.Initialize();
            }
        }
    }
}
