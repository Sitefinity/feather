using System;
using System.Linq;
using ServiceStack;
using ServiceStack.Text;
using Telerik.Sitefinity.Modules.Lists.Web.Services.Data;
using Telerik.Sitefinity.Frontend.Services.FilesService;
using Telerik.Sitefinity.Frontend.Services.FilesService.DTO;

namespace Telerik.Sitefinity.Frontend.Services.ListsService
{
    /// <summary>
    /// Represents a ServiceStack plug-in for the Files web service.
    /// </summary>
    internal class FilesServiceStackPlugin : IPlugin
    {
        /// <summary>
        /// Adding the files service routes
        /// </summary>
        /// <param name="appHost">The service stack appHost</param>
        public void Register(IAppHost appHost)
        {
            if (appHost == null)
                throw new ArgumentNullException("appHost");

            //// NOTE: exclude this property as it causes cycle loop reference exception when serializing.
            JsConfig<ListViewModel>.ExcludePropertyNames = new[] { "AvailableCultures", "ContentItem" };

            appHost.RegisterService<FilesWebService>();
            appHost.Routes.Add<FilesGetRequest>(string.Concat(FilesServiceStackPlugin.FilesServiceWebUrl, "/", "items"), ApplyTo.Get);
        }

        private const string FilesServiceWebUrl = "/files-api";
    }
}
