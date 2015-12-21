using System;
using ArtOfTest.WebAii.Core;
using Telerik.Sitefinity.TestUI.Framework.Utilities;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Framework.Wrappers.Backend.Modules
{
    /// <summary>
    /// Feather module base actions.
    /// </summary>
    public class FrontendModuleWrapper
    {
        /// <summary>
        /// Activates Feather.
        /// </summary>
        /// <param name="activeBrowser">The active browser.</param>
        public void ActivateFeather(Browser activeBrowser)
        {
            this.NavigateToModulesAndServicesPage();
            this.WaitAndRefreshDomTree(activeBrowser);

            BAT.Wrappers().Backend().ModulesAndServices().ModulesAndServicesWrapper().ActivateModule(FeatherModuleName);
            this.WaitAndRefreshDomTree(activeBrowser);
        }

        /// <summary>
        /// Installs Feather.
        /// </summary>
        /// <param name="activeBrowser">The active browser.</param>
        public void InstallFeather(Browser activeBrowser)
        {
            this.NavigateToModulesAndServicesPage();
            this.WaitAndRefreshDomTree(activeBrowser);

            BAT.Wrappers().Backend().ModulesAndServices().ModulesAndServicesWrapper().InstallModule(FeatherModuleName);
            this.WaitAndRefreshDomTree(activeBrowser);
        }

        /// <summary>
        /// Deactivates Feather.
        /// </summary>
        /// <param name="activeBrowser">The active browser.</param>
        public void DeactivateFeather(Browser activeBrowser)
        {
            this.NavigateToModulesAndServicesPage();
            this.WaitAndRefreshDomTree(activeBrowser);

            BAT.Wrappers().Backend().ModulesAndServices().ModulesAndServicesWrapper().DeactivateModule(FeatherModuleName);
            this.WaitAndRefreshDomTree(activeBrowser);
        }

        /// <summary>
        /// Uninstalls Feather.
        /// </summary>
        /// <param name="activeBrowser">The active browser.</param>
        public void UninstallFeather(Browser activeBrowser)
        {
            this.NavigateToModulesAndServicesPage();
            this.WaitAndRefreshDomTree(activeBrowser);

            BAT.Wrappers().Backend().ModulesAndServices().ModulesAndServicesWrapper().DeactivateModule(FeatherModuleName);
            this.WaitAndRefreshDomTree(activeBrowser);

            this.NavigateToModulesAndServicesPage();
            this.WaitAndRefreshDomTree(activeBrowser);

            BAT.Wrappers().Backend().ModulesAndServices().ModulesAndServicesWrapper().UninstallModule(FeatherModuleName);
            this.WaitAndRefreshDomTree(activeBrowser);
        }

        private void NavigateToModulesAndServicesPage()
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().User().EnsureAdminLoggedIn());
            RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().NavigateTo().CustomPage(ModulesAndServicesPageUrl, false));
        }

        private void WaitAndRefreshDomTree(Browser activeBrowser)
        {
            activeBrowser.WaitForAsyncJQueryRequests(AsyncJQueryRequestsInterval);
            activeBrowser.RefreshDomTree();
        }

        private const int ClientTimeoutInterval = 80000;
        private const int AsyncJQueryRequestsInterval = 10000;
        private const string FeatherModuleName = "Feather";
        private const string ModulesAndServicesPageUrl = "~/Sitefinity/Administration/ModulesAndServices";
    }
}
