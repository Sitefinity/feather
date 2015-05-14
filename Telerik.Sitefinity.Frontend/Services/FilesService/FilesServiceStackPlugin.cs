using System;
using System.Linq;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.FilesService;
using Telerik.Sitefinity.Frontend.Services.FilesService.DTO;

namespace Telerik.Sitefinity.Frontend.Services.FilesService
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

            appHost.RegisterService<FilesWebService>();
            appHost.Routes.Add<FilesGetRequest>("/files-api", ApplyTo.Get);
        }
    }
}
