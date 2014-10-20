using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class is used for package management.
    /// </summary>
    public class PackageManager
    {
        #region Public Methods

        /// <summary>
        /// Gets the current resource package.
        /// </summary>
        /// <returns>The current resource package or null if there is no package.</returns>
        public string GetCurrentPackage()
        {
            string packageName;
            var context = SystemManager.CurrentHttpContext;

            if (context == null)
                return null;

            packageName = this.GetPackageFromContext();

            if (packageName.IsNullOrEmpty())
                packageName = this.GetPackageFromUrl();

            if (packageName.IsNullOrEmpty())
                packageName = this.GetPackageFromPageInfo();

            context.Items[PackageManager.CurrentPackageKey] = packageName;

            return packageName;
        }

        /// <summary>
        /// Gets the package virtual path.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <exception cref="System.ArgumentNullException">packageName</exception>
        public string GetPackageVirtualPath(string packageName)
        {
            if (packageName.IsNullOrEmpty())
                throw new ArgumentNullException("packageName");

            var path = string.Format(System.Globalization.CultureInfo.InvariantCulture, "~/{0}/{1}", PackageManager.PackagesFolder, packageName);
            return path;
        }

        /// <summary>
        /// Gets the current package virtual path.
        /// </summary>
        /// <returns>The virtual path of the current package or null if there is no package.</returns>
        public string GetCurrentPackageVirtualPath()
        {
            var packageName = this.GetCurrentPackage();
            var packageVirtualPath = !packageName.IsNullOrEmpty() ? this.GetPackageVirtualPath(packageName) : null;
            return packageVirtualPath;
        }

        /// <summary>
        /// Enhances the given URL with information about the current package.
        /// </summary>
        /// <param name="url">The URL.</param>
        public string EnhanceUrl(string url)
        {
            if (!url.IsNullOrEmpty())
            {
                var currentPackage = this.GetCurrentPackage();
                if (!currentPackage.IsNullOrEmpty())
                {
                    return UrlTransformations.AppendParam(url, PackageUrlParameterName, currentPackage);
                }
            }

            return url;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets file name from title by stripping the incorrect characters.
        /// </summary>
        /// <param name="title">The title.</param>
        internal string StripInvalidCharacters(string title)
        {
            var result = System.Text.RegularExpressions.Regex.Replace(
                title,
                FileNameStripingRegexPattern,
                FileNameInvalidCharactersSubstitute);

            return result;
        }

        /// <summary>
        /// Gets the key of the edited container.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>That is page template Id when editing page template or message body Id when editing Email Campaigns issue.</remarks>
        /// <returns>The key.</returns>
        private static string GetEditedContainerKey(HttpContextBase context)
        {
            var requestContext = context.Items[RouteHandler.RequestContextKey] as RequestContext ?? context.Request.RequestContext;
            var keys = requestContext.RouteData.Values["Params"] as string[];

            return (keys != null && keys.Length > 0) ? keys[0] : null;
        }

        /// <summary>
        /// Gets the package from node identifier.
        /// </summary>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns></returns>
        private string GetPackageFromNodeId(string nodeId)
        {
            Guid id;
            if (!Guid.TryParse(nodeId, out id))
                return null;

            var pageManager = PageManager.GetManager();

            var pageNode = pageManager.GetPageNode(id);
            if (SystemManager.IsDesignMode)
            {
                var draft = pageManager.GetPageDraft(pageNode.PageId);
                return draft.TemplateId != Guid.Empty ? this.GetPackageFromTemplateId(draft.TemplateId.ToString()) : null;
            }

            var pageData = pageNode.GetPageData();
            return this.GetPackageFromTemplate(pageData.Template);
        }

        /// <summary>
        /// Gets the package from template identifier.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        private string GetPackageFromTemplateId(string templateId)
        {
            Guid id;
            if (!Guid.TryParse(templateId, out id))
                return null;

            var pageManager = PageManager.GetManager();
            var template = pageManager.GetTemplate(id);

            return this.GetPackageFromTemplate(template);
        }

        /// <summary>
        /// Gets the package from template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        private string GetPackageFromTemplate(PageTemplate template)
        {
            var currentTemplate = template;
            while (currentTemplate != null)
            {
                var title = currentTemplate.Title.ToString();
                var parts = title.Split('.');
                if (parts.Length > 1)
                {
                    var expectedPackageName = this.StripInvalidCharacters(parts[0]);
                    var path = HostingEnvironment.MapPath(this.GetPackageVirtualPath(expectedPackageName));
                    if (path != null && Directory.Exists(path))
                    {
                        SystemManager.CurrentHttpContext.Items[PackageManager.CurrentPackageKey] = expectedPackageName;
                        return expectedPackageName;
                    }
                }

                currentTemplate = currentTemplate.ParentTemplate;
            }

            return null;
        }

        /// <summary>
        /// Gets the package from the page template or from the Current PageSiteNode.
        /// </summary>
        /// <returns></returns>
        private string GetPackageFromPageInfo()
        {
            string packageName;
            var context = SystemManager.CurrentHttpContext;

            if (context.Items.Contains("IsTemplate") && (bool)context.Items["IsTemplate"])
            {
                var templateId = PackageManager.GetEditedContainerKey(context);
                packageName = this.GetPackageFromTemplateId(templateId);
            }
            else if (context.Request.Path.Contains("/Sitefinity/SFNwslttrs"))
            {
                var pageNodeId = PackageManager.GetEditedContainerKey(context);
                packageName = this.GetPackageFromNodeId(pageNodeId);
            }
            else
            {
                var currentNode = SiteMapBase.GetActualCurrentNode();
                packageName = currentNode != null ? this.GetPackageFromNodeId(currentNode.Id.ToString()) : null;
            }

            return packageName;
        }

        /// <summary>
        /// Gets the package from context parameters collection.
        /// </summary>
        /// <returns></returns>
        private string GetPackageFromContext()
        {
            string packageName = null;
            if (SystemManager.CurrentHttpContext.Items.Contains(PackageManager.CurrentPackageKey))
            {
                packageName = SystemManager.CurrentHttpContext.Items[PackageManager.CurrentPackageKey] as string;
            }

            return packageName;
        }

        /// <summary>
        /// Gets the package from the URL query string.
        /// </summary>
        /// <returns></returns>
        private string GetPackageFromUrl()
        {
            string packageName = SystemManager.CurrentHttpContext.Request.QueryString["package"];

            return packageName;
        }

        #endregion

        #region Constants

        /// <summary>
        /// The folder where packages are located.
        /// </summary>
        public const string PackagesFolder = "ResourcePackages";

        /// <summary>
        /// The regex pattern for stripping file names.
        /// </summary>
        public const string FileNameStripingRegexPattern = @"[\\/><\:\?\""\*|]+|\.+$";

        /// <summary>
        /// The file name incorrect characters substitute
        /// </summary>
        public const string FileNameInvalidCharactersSubstitute = "_";

        /// <summary>
        /// The current package key
        /// </summary>
        public const string CurrentPackageKey = "CurrentResourcePackage";

        public const string PackageUrlParameterName = "package";

        #endregion
    }
}
