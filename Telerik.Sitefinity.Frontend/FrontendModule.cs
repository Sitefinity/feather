using System;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
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
    /// A module that will be invoked by Sitefinity.
    /// </summary>
    public class FrontendModule : ModuleBase
    {
        /// <summary>
        /// Gets the landing page id for each module inherit from <see cref="SecuredModuleBase"/> class.
        /// </summary>
        /// <value>The landing page id.</value>
        public override Guid LandingPageId
        {
            get { return Guid.Empty; }
        }

        /// <summary>
        /// Gets the CLR types of all data managers provided by this module.
        /// </summary>
        /// <value>An array of <see cref="Type"/> objects.</value>
        public override Type[] Managers
        {
            get { return new Type[0]; }
        }

        /// <summary>
        /// Installs the specified initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public override void Install(SiteInitializer initializer)
        {
            //// No installation needed.
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
        /// Gets the module configuration.
        /// </summary>
        protected override ConfigSection GetModuleConfig()
        {
            return null;
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
                var resourcesInitializer = new ResourcesInitializer();
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
