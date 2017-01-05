using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Http;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.TestIntegration.Helpers;
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
            {
                this.ActivateFeather();
                if (this.IsFeatherDisabled())
                    throw new ArgumentException("Feather module must be installed to run this test");
            }
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
            var url = "/Sitefinity/Services/ModulesService/modules?operation=2";
            var installOperationEndpoint = UrlPath.ResolveUrl(url, true);
            var json = "{\"ClientId\":\"Feather\",\"Description\":\"Modern, intuitive, convention based, mobile-first UI for Telerik Sitefinity\",\"ErrorMessage\":\"\",\"IsModuleLicensed\":true,\"IsSystemModule\":false,\"Key\":\"Feather\",\"ModuleId\":\"00000000-0000-0000-0000-000000000000\",\"ModuleType\":0,\"Name\":\"Feather\",\"ProviderName\":\"\",\"StartupType\":3,\"Status\":1,\"Title\":\"Feather\",\"Type\":\"Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend\",\"Version\":{\"_Build\":400,\"_Major\":1,\"_Minor\":4,\"_Revision\":0}}";
            this.MakePutRequest(installOperationEndpoint, json);

            Thread.Sleep(10000);
        }

        /// <summary>
        /// Installs Feather.
        /// </summary>
        public void InstallFeather()
        {
            var url = "/Sitefinity/Services/ModulesService/modules?operation=0";
            var installOperationEndpoint = UrlPath.ResolveUrl(url, true);
            var json = "{\"ClientId\":\"Feather\",\"Description\":\"Modern, intuitive, convention based, mobile-first UI for Telerik Sitefinity\",\"ErrorMessage\":\"\",\"IsModuleLicensed\":true,\"IsSystemModule\":false,\"Key\":\"Feather\",\"ModuleId\":\"00000000-0000-0000-0000-000000000000\",\"ModuleType\":0,\"Name\":\"Feather\",\"ProviderName\":\"\",\"StartupType\":0,\"Status\":0,\"Title\":\"Feather\",\"Type\":\"Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend\",\"Version\":null}";
            this.MakePutRequest(installOperationEndpoint, json);

            Thread.Sleep(10000);
        }

        /// <summary>
        /// Deactivates Feather.
        /// </summary>
        public void DeactivateFeather()
        {
            var url = "/Sitefinity/Services/ModulesService/modules?operation=3";
            var uninstallOperationEndpoint = UrlPath.ResolveUrl(url, true);
            var json = "{\"ClientId\":\"Feather\",\"Description\":\"Modern, intuitive, convention based, mobile-first UI for Telerik Sitefinity\",\"ErrorMessage\":\"\",\"IsModuleLicensed\":true,\"IsSystemModule\":false,\"Key\":\"Feather\",\"ModuleId\":\"00000000-0000-0000-0000-000000000000\",\"ModuleType\":0,\"Name\":\"Feather\",\"ProviderName\":\"\",\"StartupType\":0,\"Status\":2,\"Title\":\"Feather\",\"Type\":\"Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend\",\"Version\":{\"_Build\":400,\"_Major\":1,\"_Minor\":4,\"_Revision\":0}}";
            this.MakePutRequest(uninstallOperationEndpoint, json);

            Thread.Sleep(10000);
        }

        /// <summary>
        /// Uninstalls Feather.
        /// </summary>
        public void UninstallFeather()
        {
            var url = "/Sitefinity/Services/ModulesService/modules?operation=1";
            var uninstallOperationEndpoint = UrlPath.ResolveUrl(url, true);
            var json = "{\"ClientId\":\"Feather\",\"Description\":\"Modern, intuitive, convention based, mobile-first UI for Telerik Sitefinity\",\"ErrorMessage\":\"\",\"IsModuleLicensed\":true,\"IsSystemModule\":false,\"Key\":\"Feather\",\"ModuleId\":\"00000000-0000-0000-0000-000000000000\",\"ModuleType\":0,\"Name\":\"Feather\",\"ProviderName\":\"\",\"StartupType\":3,\"Status\":1,\"Title\":\"Feather\",\"Type\":\"Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend\",\"Version\":{\"_Build\":400,\"_Major\":1,\"_Minor\":4,\"_Revision\":0}}";
            this.MakePutRequest(uninstallOperationEndpoint, json);

            Thread.Sleep(10000);
        }

        private void MakePutRequest(string url, string payload)
        {
            var client = new SitefinityClient();
            client.RequestAuthenticate();
            var request = new HttpRequestMessage("put", url);
            
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(payload);
            request.Headers.ContentType = "text/json";
            request.Headers.ContentLength = bytes.Length;
            request.Content = HttpContent.Create(bytes);
            var response = client.Send(request);
        }
    }
}
