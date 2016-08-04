using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using Telerik.Sitefinity.Restriction;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations
{
    /// <summary>
    /// Provides common feather module operations
    /// </summary>
    public class FeatherModuleOperations
    {        
        private ModuleViewModelProxy GetFeatherModule
        {
            get
            {
                if (this.featherSettings == null)
                {
                    this.featherSettings = new ModuleViewModelProxy();
                    this.featherSettings.ClientId = "Feather";
                    this.featherSettings.Key = "Feather";
                    this.featherSettings.Type = "Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend";
                    this.featherSettings.ModuleId = new Guid("00000000-0000-0000-0000-000000000000"); 
                    this.featherSettings.Name = "Feather";

                    return this.featherSettings;
                }

                return this.featherSettings;
            }
        }

        /// <summary>
        /// Ensures the feather enabled.
        /// </summary>
        /// <exception cref="System.ArgumentException">Feather module must be installed to run this test</exception>
        public void EnsureFeatherEnabled()
        {
            if (this.IsFeatherDisabled())
                throw new ArgumentException("Feather module must be installed to run this test");
        }

        /// <summary>
        /// Determines whether feather module is disabled.
        /// </summary>
        /// <returns></returns>
        public bool IsFeatherDisabled()
        {
            var isDisabled = SystemManager.GetModule("Feather") == null;
            return isDisabled;
        }

        /// <summary>
        /// Activates Feather.
        /// </summary>
        public void ActivateFeather()
        {
            using (new UnrestrictedModeRegion())
            {
                var module = this.GetFeatherModule;
                ServerOperations.StaticModules().ExecuteStaticModuleOperation(module, ModuleOperationProxy.Activate);
            }
        }

        /// <summary>
        /// Installs Feather.
        /// </summary>
        public void InstallFeather()
        {
            using (new UnrestrictedModeRegion())
            {
                var module = this.GetFeatherModule;
                ServerOperations.StaticModules().ExecuteStaticModuleOperation(module, ModuleOperationProxy.Install);
            }
        }

        /// <summary>
        /// Deactivates Feather.
        /// </summary>
        public void DeactivateFeather()
        {
            using (new UnrestrictedModeRegion())
            {
                var module = this.GetFeatherModule;
                ServerOperations.StaticModules().ExecuteStaticModuleOperation(module, ModuleOperationProxy.Deactivate);
            }
        }

        /// <summary>
        /// Uninstalls Feather.
        /// </summary>
        public void UninstallFeather()
        {
            using (new UnrestrictedModeRegion())
            {
                var module = this.GetFeatherModule;
                ServerOperations.StaticModules().ExecuteStaticModuleOperation(module, ModuleOperationProxy.Uninstall);
            }
        }

        private ModuleViewModelProxy featherSettings;
    }
}
