using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.FilesService.DTO;
using Telerik.Sitefinity.Web.Services;

namespace Telerik.Sitefinity.Frontend.Services.FilesService
{
    /// <summary>
    /// This class provides file retrieval by extension.
    /// </summary>
    internal class FilesWebService : Service
    {
        #region Actions

        /// <summary>
        /// Gets specific directories and files depending on the requested file extension and parent path.
        /// </summary>
        /// <param name="filesRequest">The files requests object from which the request parameters to be retrieved.</param>
        /// <returns>        
        /// An <see cref="FilesViewModel"/> object.
        /// </returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public FilesViewModel Get(FilesGetRequest filesRequest)
        {
            ServiceUtility.RequestBackendUserAuthentication();

            var result = new FilesViewModel();

            string error;
            if (this.ValidateFilesRequest(filesRequest, out error))
            {
                result.Items = this.GetChildItems(filesRequest);
            }
            else
            {
                result.Error = error;
            }

            ServiceUtility.DisableCache();

            return result;
        }

        #endregion

        #region Items Retrieval

        private IEnumerable<FilesItemViewModel> GetChildItems(FilesGetRequest filesRequest)
        {
            var parentPath = PathUtils.CombinePaths(this.GetApplicationRootPath(), filesRequest.Path);

            var dirs = Directory.GetDirectories(parentPath).Select(d => new FilesItemViewModel() { IsFolder = true, Name = d.Substring(parentPath.Length), HasChildren = !this.IsDirectoryEmpty(d, filesRequest.Extension) }).OrderBy(d => d.Name);

            var files = Directory.GetFiles(parentPath, string.Format("*.{0}", filesRequest.Extension)).Select(f => new FilesItemViewModel() { Name = f.Substring(parentPath.Length).TrimStart('\\') }).OrderBy(f => f.Name);

            return dirs.Union(files).Skip(filesRequest.Skip).Take(filesRequest.Take == 0 ? FilesWebServiceConstants.MaxItemsPerRequest : filesRequest.Take);
        }

        private bool IsDirectoryEmpty(string path, string extension)
        {
            return !Directory.GetDirectories(path).Any() && !Directory.GetFiles(path, string.Format("*.{0}", extension)).Any();
        }

        #endregion

        #region Validation

        private bool ValidateFilesRequest(FilesGetRequest filesRequest, out string error)
        {
            var isValid = this.ValidateExtension(filesRequest.Extension, out error)
                && this.ValidatePath(filesRequest.Path, out error)
                && this.ValidateSkipTake(filesRequest.Skip, filesRequest.Take, out error);

            return isValid;
        }

        private bool ValidateExtension(string extension, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(extension))
            {
                error = FilesWebServiceConstants.FileExtensionNullOrEmptyExceptionMessage;
            }
            else if (!this.GetAllowedFileExtensions().Contains(extension))
            {
                error = string.Format(FilesWebServiceConstants.FileExtensionNotSupportedExceptionMessageFormat, extension);
            }

            return string.IsNullOrEmpty(error);
        }

        private bool ValidatePath(string path, out string error)
        {
            error = null;

            if (!string.IsNullOrEmpty(path))
            {
                var forbidenSymbolInPath = this.GetForbiddenPathSymbols().FirstOrDefault(s => path.Contains(s));
                if (forbidenSymbolInPath != null)
                {
                    error = string.Format(FilesWebServiceConstants.ParentPathForbiddenSymbolInPathExceptionMessageFormat, path, forbidenSymbolInPath);
                }
                else if (!Directory.Exists(PathUtils.CombinePaths(this.GetApplicationRootPath(), path)))
                {
                    error = string.Format(FilesWebServiceConstants.ParentPathNotExistingExceptionMessageFormat, path);
                }
            }

            return string.IsNullOrEmpty(error);
        }

        private bool ValidateSkipTake(int skip, int take, out string error)
        {
            error = null;

            if (skip < 0)
            {
                error = FilesWebServiceConstants.FilesSkipNegativeValueExceptionMessage;
            }
            else if (take > FilesWebServiceConstants.MaxItemsPerRequest)
            {
                error = string.Format(FilesWebServiceConstants.FilesTakeMaxLimitExceptionMessageFormat, FilesWebServiceConstants.MaxItemsPerRequest);
            }
            else if (take < 0)
            {
                error = FilesWebServiceConstants.FilesTakeNegativeValueExceptionMessage;
            }

            return string.IsNullOrEmpty(error);
        }

        #endregion

        #region Private Methods

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
            return new string[] { "..", "~" };
        }

        #endregion
    }
}
