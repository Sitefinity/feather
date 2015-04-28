using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Telerik.Sitefinity.Frontend.Services.FilesService.DTO;
using Telerik.Sitefinity.Modules.Lists.Web.Services.Data;
using Telerik.Sitefinity.Web.Services;
using Telerik.Sitefinity.Utilities.MS.ServiceModel.Web;
using System.Net;

namespace Telerik.Sitefinity.Frontend.Services.FilesService
{
    /// <summary>
    /// This class provides file retrieval by extension.
    /// </summary>
    internal class FilesWebService : Service
    {
        [AddHeader(ContentType = MimeTypes.Json)]
        public CollectionContext<ListViewModel> Get(FilesGetRequest filesRequest)
        {
            ServiceUtility.RequestBackendUserAuthentication();

            if (filesRequest == null)
            {
                filesRequest = new FilesGetRequest() { ParentPath = null, Skip = 0, Take = FilesWebService.MaxItemsPerRequest };
            }
            else
            {
                ValidateFilesRequest(filesRequest);
            }

            var result = this.GetChildItems(filesRequest.FileExtension, 

            ServiceUtility.DisableCache();
            
            return new CollectionContext<ListViewModel>(result) { TotalCount = result.Count };
        }

        private string GetApplicationRootPath()
        {
            return HostingEnvironment.ApplicationPhysicalPath;
        }

        private IEnumerable<string> GetAllowedFileExtensions()
        {
            return new string[] { "css", "js", "html" };
        }

        private IEnumerable<string> GetForbiddenPathSymbols()
        {
            return new string[] { ".." };
        }

        private IEnumerable<FilesViewModel> GetChildItems(string fileExtension, string parentPath, int skip, int take)
        {
            
        }

        private void ValidateFilesRequest(FilesGetRequest filesRequest)
        {
            if (string.IsNullOrEmpty(filesRequest.FileExtension))
            {
                throw new WebServiceException(string.Format("File extension can not be null or empty.", filesRequest.ParentPath)) { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            if (!this.GetAllowedFileExtensions().Contains(filesRequest.FileExtension))
            {
                throw new WebServiceException(string.Format("File extension {0} is not supported.", filesRequest.FileExtension)) { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            var forbidenSymbolInPath = string.IsNullOrEmpty(filesRequest.ParentPath) ? null : this.GetForbiddenPathSymbols().FirstOrDefault(s => filesRequest.ParentPath.Contains(s));
            if (forbidenSymbolInPath != null)
            {
                throw new WebServiceException(string.Format("Parent path {0} contains unallowed symbols ({1}).", filesRequest.ParentPath, forbidenSymbolInPath)) { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            if (filesRequest.Take > FilesWebService.MaxItemsPerRequest)
            {
                throw new WebServiceException(string.Format("Can not request more than {0} items.", FilesWebService.MaxItemsPerRequest)) { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }

        private const int MaxItemsPerRequest = 50;
    }
}
