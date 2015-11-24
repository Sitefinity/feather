using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations
{
    /// <summary>
    /// Provides common feather module operations
    /// </summary>
    public class FeatherModuleOperations
    {
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
        /// Activates the state of the feather from deactivated state.
        /// </summary>
        public void ActivateFeatherFromDeactivatedState()
        {
            var installOperationEndpoint = UrlPath.ResolveUrl(FeatherModuleOperations.FeatherActivateFromDeactivatedStateUrl, true);
            this.MakePutRequest(installOperationEndpoint, JsonRequestPayload);
        }

        /// <summary>
        /// Activates the state of the feather from uninstalled state.
        /// </summary>
        public void ActivateFeatherFromUninstalledState()
        {
            var installOperationEndpoint = UrlPath.ResolveUrl(FeatherModuleOperations.FeatherActivateFromUninstalledStateUrl, true);
            this.MakePutRequest(installOperationEndpoint, JsonRequestPayload);
        }

        /// <summary>
        /// Deactivates the feather.
        /// </summary>
        public void DeactivateFeather()
        {
            var uninstallOperationEndpoint = UrlPath.ResolveUrl(FeatherModuleOperations.FeatherDeactivateUrl, true);
            this.MakePutRequest(uninstallOperationEndpoint, JsonRequestPayload);
        }

        /// <summary>
        /// Uninstalls the feather.
        /// </summary>
        public void UninstallFeather()
        {
            var uninstallOperationEndpoint = UrlPath.ResolveUrl(FeatherModuleOperations.FeatherUninstallUrl, true);
            this.MakePutRequest(uninstallOperationEndpoint, JsonRequestPayload);
        }

        private void MakePutRequest(string url, string payload)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "PUT";

            using (var writer = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                writer.Write(payload);
            }

            var response = httpWebRequest.GetResponse();
        }

        private const string FeatherUninstallUrl = "/Sitefinity/Services/ModulesService/modules?operation=1";
        private const string FeatherActivateFromDeactivatedStateUrl = "/Sitefinity/Services/ModulesService/modules?operation=2";
        private const string FeatherActivateFromUninstalledStateUrl = "/Sitefinity/Services/ModulesService/modules?operation=0";
        private const string FeatherDeactivateUrl = "/Sitefinity/Services/ModulesService/modules?operation=3";
        private const string JsonRequestPayload = "{\"ClientId\":\"Feather\",\"Description\":\"Modern, intuitive, convention based, mobile-first UI for Telerik Sitefinity\",\"ErrorMessage\":\"\",\"IsModuleLicensed\":true,\"IsSystemModule\":false,\"Key\":\"Feather\",\"ModuleId\":\"00000000-0000-0000-0000-000000000000\",\"ModuleType\":0,\"Name\":\"Feather\",\"ProviderName\":\"\",\"StartupType\":3,\"Status\":1,\"Title\":\"Feather\",\"Type\":\"Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend\",\"Version\":{\"_Build\":390,\"_Major\":1,\"_Minor\":4,\"_Revision\":0}}";
    }
}
