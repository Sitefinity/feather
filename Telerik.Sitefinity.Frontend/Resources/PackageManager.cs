using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.HealthMonitoring;
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

            if (context.Request.Path.Contains("/Sitefinity/Versioning"))
            {
                // If we are in the versioning try get tha package from page info
                if (SystemManager.CurrentHttpContext.Items.Contains(PackageManager.CurrentVersionTemplateId))
                {
                    var templateId = SystemManager.CurrentHttpContext.Items[PackageManager.CurrentVersionTemplateId] as string;
                    packageName = this.GetPackageFromTemplateId(templateId);
                    if (!packageName.IsNullOrEmpty())
                        return packageName;
                }
            }

            packageName = this.GetPackageFromContext();
            if (!packageName.IsNullOrEmpty())
                return packageName;

            if (packageName.IsNullOrEmpty() && this.AllowChangePackageAtRuntimeViaQueryString)
                packageName = this.GetPackageFromUrl();

            if (packageName.IsNullOrEmpty())
                packageName = this.GetPackageFromPageInfo();

            if (packageName.IsNullOrEmpty() || this.PackageExists(packageName))
            {
                context.Items[PackageManager.CurrentPackageKey] = packageName;

                return packageName;
            }
            else
            {
                return null;
            }
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

        /// <summary>
        /// Checks whether a given package exists.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        public virtual bool PackageExists(string packageName)
        {
            var path = HostingEnvironment.MapPath(this.GetPackageVirtualPath(packageName));
            return path != null && Directory.Exists(path);
        }

        /// <summary>
        /// Gets a value indicating whether the package manager will try to resolve the package from URL.
        /// </summary>
        protected virtual bool AllowChangePackageAtRuntimeViaQueryString
        {
            get
            {
                return Telerik.Sitefinity.Abstractions.AppSettings.CurrentSettings.IsBackend || 
                    Config.Get<Sitefinity.Modules.Pages.Configuration.PagesConfig>().AllowChangePageThemeAtRuntime ||
                    (SystemManager.CurrentHttpContext.Items[SiteMapBase.CurrentNodeKey] as PageSiteNode) == null;
            }
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
            if (keys != null && keys.Length > 0)
            {
                return keys[0];
            }
            else if (requestContext.RouteData.Values.ContainsKey("itemId"))
            {
                return requestContext.RouteData.Values["itemId"] as string;
            }
            else
            {
                return null;
            }
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
                var name = currentTemplate.Name ?? (currentTemplate.Title != null ? currentTemplate.Title.ToString() : null);
                if (!name.IsNullOrEmpty())
                {
                    var parts = name.Split('.');
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
                }

                currentTemplate = currentTemplate.ParentTemplate;
            }

            return null;
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
            var pageData = pageNode.GetPageData();

            if (pageData == null)
            {
                return null;
            }

            if (SystemManager.IsDesignMode)
            {
                var draft = pageManager.GetPageDraft(pageData.Id);
                if (draft != null)
                {
                    return draft.TemplateId != Guid.Empty ? this.GetPackageFromTemplateId(draft.TemplateId.ToString()) : null;
                }
            }

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
        /// Gets the package from the page template or from the Current PageSiteNode.
        /// </summary>
        /// <returns></returns>
        private string GetPackageFromPageInfo()
        {
            string packageName = null;
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
            else if (context.Request.Path.Contains("/Sitefinity/Versioning"))
            {
                var pageNodeId = new Guid(context.Request.RequestContext.RouteData.Values["itemId"].ToString());
                var page = PageManager.GetManager().GetPageData(pageNodeId);

                if (page.NavigationNodeId != Guid.Empty)
                {
                    packageName = this.GetPackageFromNodeId(page.NavigationNodeId.ToString());
                }

                if (packageName == null)
                {
                    packageName = this.GetPackageFromTemplate(page.Template);
                }
            }
            else
            {
                var requestContext = context.Request.RequestContext;
                var currentNode = requestContext.RouteData.DataTokens["SiteMapNode"] as PageSiteNode;

                if (currentNode == null)
                    currentNode = SiteMapBase.GetActualCurrentNode();

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
            string packageName = SystemManager.CurrentHttpContext.Request.QueryStringGet(
                "package",
                () => { return new PackageQueryParameterValidator() { Arguments = !this.GetType().Equals(typeof(PackageManager)) ? new string[] { this.GetType().Assembly.FullName, this.GetType().FullName } : null }; });
            return packageName;
        }

        #endregion

        [Serializable]
        class PackageQueryParameterValidator : CacheVariationParamValidator
        {
            protected override bool Validate(string paramValue, string[] arguments)
            {
                PackageManager packageManager = null;
                if (arguments != null && arguments.Length == 2)
                {
                    try
                    {
                        var assembly = Assembly.Load(arguments[0]);
                        if (assembly != null)
                            packageManager = assembly.CreateInstance(arguments[1]) as PackageManager;
                    }
                    catch
                    {
                    }
                }

                if (packageManager == null)
                {
                    packageManager = new PackageManager();
                }

                return packageManager.PackageExists(paramValue);
            }
        }


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

        public const string CurrentVersionTemplateId = "CurrentVersionTemplateId";

        public const string PackageUrlParameterName = "package";

        #endregion
    }
}
