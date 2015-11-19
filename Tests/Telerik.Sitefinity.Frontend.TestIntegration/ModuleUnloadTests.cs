using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using MbUnit.Framework;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration
{
    /// <summary>
    /// This class contains tests for unloading of the Feather module.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for unloading of the Feather module.")]
    public class ModuleUnloadTests
    {
        /// <summary>
        /// Checks whether after deactivating Feather the Sitefinity application changes it has done are undone.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the Sitefinity application changes it has done are undone.")]
        public void DeactivatingFeatherShouldRestoreAppStateToPreFeatherActivation()
        {
            //// var featherWasEnabled = this.IsFeatherEnabled();

            //// try
            //// {
                //// if (!featherWasEnabled)
                    //// this.ActivateFeather();

                //// this.DeactivateFeather();

                this.UnloadingFeather_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone();
                this.UnloadingFeather_SystemManager_RegisterServiceStackPlugin_ShouldBeUndone();
                this.UnloadingFeather_ObjectFactory_Container_RegisterType_ShouldBeUndone();
                this.UnloadingFeather_SystemManager_RegisterRoute_ShouldBeUndone();
                this.UnloadingFeather_EventHub_Subscribe_ShouldBeUndone();
                this.UnloadingFeather_Res_RegisterResource_ShouldBeUndone();
                this.UnloadingFeather_IFileMonitor_Start_ShouldBeUndone();
                this.UnloadingFeather_GlobalFilters_Filters_Add_ShouldBeUndone();
                this.UnloadingFeather_ControlTemplates_RegisterTemplatableControl_ShouldBeUndone();
                this.UnloadingFeather_ViewEngines_Engines_Remove_ShouldBeUndone();
                this.UnloadingFeather_ControllerBuilder_Current_SetControllerFactory_ShouldBeUndone();
                this.UnloadingFeather_ControllerStore_AddController_ShouldBeUndone();
                this.UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone();
                this.UnloadingFeather_RouteTable_Routes_MapRoute_ShouldBeUndone();
            //// }
            //// finally
            //// {
                //// if (featherWasEnabled)
                    //// this.ActivateFeather();
            //// }
        }

        /// <summary>
        /// Checks whether after unloading Feather the VirtualPathManager AddVirtualFileResolver calls are undone.
        /// </summary>
        public void UnloadingFeather_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the SystemManager RegisterServiceStackPlugin calls are undone.
        /// </summary>
        public void UnloadingFeather_SystemManager_RegisterServiceStackPlugin_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the ObjectFactory Container RegisterType calls are undone.
        /// </summary>
        public void UnloadingFeather_ObjectFactory_Container_RegisterType_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the SystemManager RegisterRoute calls are undone.
        /// </summary>
        public void UnloadingFeather_SystemManager_RegisterRoute_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the EventHub Subscribe calls are undone.
        /// </summary>
        public void UnloadingFeather_EventHub_Subscribe_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the Res RegisterResource calls are undone.
        /// </summary>
        public void UnloadingFeather_Res_RegisterResource_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the IFileMonitor Start calls are undone.
        /// </summary>
        public void UnloadingFeather_IFileMonitor_Start_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the GlobalFilters Filters Add calls are undone.
        /// </summary>
        public void UnloadingFeather_GlobalFilters_Filters_Add_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the ControlTemplates RegisterTemplatableControl calls are undone.
        /// </summary>
        public void UnloadingFeather_ControlTemplates_RegisterTemplatableControl_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the ViewEngines Engines Remove calls are undone.
        /// </summary>
        public void UnloadingFeather_ViewEngines_Engines_Remove_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the ControllerBuilder Current SetControllerFactory calls are undone.
        /// </summary>
        public void UnloadingFeather_ControllerBuilder_Current_SetControllerFactory_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the ControllerStore AddController calls are undone.
        /// </summary>
        public void UnloadingFeather_ControllerStore_AddController_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the RouteTable Routes Insert calls are undone.
        /// </summary>
        public void UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone()
        {
        }

        /// <summary>
        /// Checks whether after unloading Feather the RouteTable Routes MapRoute calls are undone.
        /// </summary>
        public void UnloadingFeather_RouteTable_Routes_MapRoute_ShouldBeUndone()
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private bool IsFeatherEnabled()
        {
            return SystemManager.GetModule("Feather") != null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ActivateFeather()
        {
            if (this.IsFeatherEnabled())
                return;

            var installOperationEndpoint = UrlPath.ResolveUrl("/Sitefinity/Services/ModulesService/modules?operation=2", true);
            this.MakePutRequest(installOperationEndpoint, JsonRequestPayload);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void DeactivateFeather()
        {
            if (!this.IsFeatherEnabled())
                return;

            var uninstallOperationEndpoint = UrlPath.ResolveUrl("/Sitefinity/Services/ModulesService/modules?operation=3", true);
            this.MakePutRequest(uninstallOperationEndpoint, JsonRequestPayload);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void MakePutRequest(string url, string payload)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.Headers["Cookie"] = HttpContext.Current.Request.Headers["Cookie"];
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "PUT";

            using (var writer = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                writer.Write(payload);
            }

            httpWebRequest.GetResponse();
        }

        private const string JsonRequestPayload = "{\"ClientId\":\"Feather\",\"Description\":\"Modern, intuitive, convention based, mobile-first UI for Telerik Sitefinity\",\"ErrorMessage\":\"\",\"IsModuleLicensed\":true,\"IsSystemModule\":false,\"Key\":\"Feather\",\"ModuleId\":\"00000000-0000-0000-0000-000000000000\",\"ModuleType\":0,\"Name\":\"Feather\",\"ProviderName\":\"\",\"StartupType\":3,\"Status\":1,\"Title\":\"Feather\",\"Type\":\"Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend\",\"Version\":{\"_Build\":390,\"_Major\":1,\"_Minor\":4,\"_Revision\":0}}";
    }
}
