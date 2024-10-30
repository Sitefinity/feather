using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Resources.Resolvers
{
    /// <summary>
    /// This class implements a resource resolver node that gets resources from the database.
    /// </summary>
    internal class DatabaseResourceResolver : ResourceResolverNode
    {
        /// <inheritdoc />
        protected override bool CurrentExists(PathDefinition definition, string virtualPath)
        {
            var cacheManager = this.GetCacheManager();
            var key = this.GetExistsCacheKey(definition, virtualPath);
            bool? result = cacheManager[key] as bool?;
            if (result == null)
            {
                lock (this.existsLock)
                {
                    result = cacheManager[key] as bool?;
                    if (result == null)
                    {
                        bool isValidControlPresentationPath;
                        ControlPresentation controlPresentation = this.GetControlPresentation(definition, virtualPath, out isValidControlPresentationPath);
                        result = controlPresentation != null && !controlPresentation.Data.IsNullOrEmpty();

                        cacheManager.Add(key, result, Microsoft.Practices.EnterpriseLibrary.Caching.CacheItemPriority.Normal, null, this.GetControlPresentationsCacheExpirations());
                    }
                }
            }

            return result.Value;
        }

        /// <inheritdoc />
        protected override System.Web.Caching.CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            bool isValidControlPresentationPath;
            string name;
            IEnumerable<string> controllers;
            string[] areaNames;

            ControlPresentationCacheDependency cacheDependency = null;
            if (this.TryResolveControlPresentationParams(definition, virtualPath, out isValidControlPresentationPath, out name, out controllers, out areaNames)
                && isValidControlPresentationPath)
            {
                foreach (string areaName in areaNames)
                {
                    var item = this.GetControlPresentationItem(controllers, name, areaName);
                    if (item != null)
                    {
                        cacheDependency = new ControlPresentationCacheDependency(item.Id.ToString());
                        break;
                    }
                }

                if (cacheDependency == null)
                {
                    List<CacheDependencyKey> keys = new List<CacheDependencyKey>();
                    foreach (string areaName in areaNames)
                    {
                        foreach (var controller in controllers)
                        {
                            var key = string.Concat(name, controller, areaName);
                            if (cacheDependency == null)
                                cacheDependency = new ControlPresentationCacheDependency(key);
                            else
                                cacheDependency.AddAdditionalKey(key);
                        }
                    }
                }
            }

            return cacheDependency;
        }

        /// <inheritdoc />
        protected override Stream CurrentOpen(PathDefinition definition, string virtualPath)
        {
            bool isValidControlPresentationPath;
            ControlPresentation controlPresentation = this.GetControlPresentation(definition, virtualPath, out isValidControlPresentationPath);
            if (controlPresentation != null && !controlPresentation.Data.IsNullOrEmpty())
            {
                var bytes = RouteHelper.GetContentWithPreamble(controlPresentation.Data);
                return new MemoryStream(bytes);
            }

            throw new ArgumentException("Could not find resource at " + virtualPath + " in the database.");
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Telerik.Sitefinity", "SF1001:AvoidToListOnIQueryable"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        protected override IEnumerable<string> GetCurrentFiles(PathDefinition definition, string path)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            if (path == null)
                throw new ArgumentNullException("path");

            var cacheManager = this.GetCacheManager();
            var key = this.GetFilesCacheKey(definition, path);
            var result = cacheManager[key] as IEnumerable<string>;
            if (result == null)
            {
                lock (this.getFilesLock)
                {
                    result = cacheManager[key] as IEnumerable<string>;
                    if (result == null)
                    {
                        var controllerName = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

                        if (controllerName == null)
                            return null;

                        var controllers = this.GetControllersFullNames(definition);

                        if (controllers == null)
                            return null;

                        string moduleName = definition.Parameters[ModuleNameParam];
                        string areaName = this.GetAreaName(controllerName, moduleName);

                        result = this.GetViewPaths(path, controllers, areaName);

                        cacheManager.Add(key, result, Microsoft.Practices.EnterpriseLibrary.Caching.CacheItemPriority.Normal, null, this.GetControlPresentationsCacheExpirations());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the control presentation that contains the requested resource from the database.
        /// </summary>
        /// <param name="virtualPathDefinition">The virtual path definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="isValidControlPresentationPath">Value indicating whether virtualPath is valid control presentation path.</param>
        /// <returns>
        /// The control presentation for the given path.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        protected virtual ControlPresentation GetControlPresentation(PathDefinition virtualPathDefinition, string virtualPath, out bool isValidControlPresentationPath)
        {
            string name;
            IEnumerable<string> controllers;
            string[] areaNames;
            if (this.TryResolveControlPresentationParams(virtualPathDefinition, virtualPath, out isValidControlPresentationPath, out name, out controllers, out areaNames))
            {
                foreach (string areaName in areaNames)
                {
                    var item = this.GetControlPresentationItem(controllers, name, areaName);
                    if (item == null) continue;
                    return item;
                }
            }

            return null;
        }

        protected virtual bool TryResolveControlPresentationParams(
            PathDefinition virtualPathDefinition,
            string virtualPath,
            out bool isValidPath,
            out string name,
            out IEnumerable<string> controllers,
            out string[] areaNames)
        {
            if (virtualPathDefinition == null)
                throw new ArgumentNullException("virtualPathDefinition");

            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            isValidPath = false;
            name = null;
            controllers = null;
            areaNames = null;

            var extension = Path.GetExtension(virtualPath);

            /// TODO: Fix - currently allowed only for razor views
            if (extension == MvcConstants.RazorFileNameExtension)
            {
                name = Path.GetFileNameWithoutExtension(virtualPath);
                controllers = this.GetControllersFullNames(virtualPathDefinition);
                var pathNames = virtualPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (controllers == null || pathNames == null)
                {
                    return false;
                }

                if (Regex.IsMatch(virtualPath, DatabaseResourceResolver.ControlPresentationViewPathPattern, RegexOptions.IgnoreCase))
                {
                    isValidPath = true;
                }

                string controllerName;
                if (pathNames.Length > 0)
                    controllerName = pathNames[pathNames.Length - 2];
                else
                    controllerName = string.Empty;

                areaNames = this.FindAreaNames(controllerName, null);
                return true;
            }

            return false;

        }

        /// <summary>
        /// Gets the cache manager.
        /// </summary>
        /// <returns></returns>
        protected virtual Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager GetCacheManager()
        {
            return SystemManager.GetCacheManager(CacheManagerInstance.Global);
        }

        /// <summary>
        /// Gets the cache item expiration objects for control presentation data items.
        /// </summary>
        /// <returns>The cache item expirations.</returns>
        protected virtual Microsoft.Practices.EnterpriseLibrary.Caching.ICacheItemExpiration[] GetControlPresentationsCacheExpirations()
        {
            return new[] { new DataItemCacheDependency(typeof(ControlPresentation), null) };
        }

        /// <summary>
        /// Gets the view paths available for current widget.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="controllers">The controllers.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>available view paths</returns>
        private IEnumerable<string> GetViewPaths(string path, IEnumerable<string> controllers, string areaName)
        {
            var viewPaths = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(t => controllers.Contains(t.ControlType) && t.AreaName == areaName)
                                            .Select(t => string.Format(CultureInfo.InvariantCulture, DatabaseResourceResolver.ViewPathTemplate, path, t.Name))
                                            .ToArray();

            return viewPaths;
        }

        /// <summary>
        /// Gets <see cref="ControlPresentation"/> for specific name and area name,
        /// containg the cpecified <see cref="Controller"/> full names.
        /// </summary>
        /// <param name="controllers">The controllers.</param>
        /// <param name="name">The name.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <returns><see cref="ControlPresentation"/> item.</returns>
        protected virtual ControlPresentation GetControlPresentationItem(IEnumerable<string> controllers, string name, string areaName)
        {
            var returnResult = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(t => controllers.Contains(t.ControlType))
                                            .FirstOrDefault(cp => cp.Name == name && cp.AreaName == areaName);

            return returnResult;
        }

        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="moduleName">Name of dynamic module.</param>
        /// <returns></returns>
        private string GetAreaName(string controllerName, string moduleName)
        {
            return this.FindAreaNames(controllerName, moduleName).FirstOrDefault();
        }

        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="moduleName">Name of dynamic module.</param>
        /// <returns></returns>
        private string[] FindAreaNames(string controllerName, string moduleName)
        {
            var dynamicTypes = ControllerExtensions.FindDynamicContentTypes(controllerName, moduleName).ToArray();
            List<string> areaNames = new List<string>();

            // case for dynamic types
            if (dynamicTypes.Length > 0)
            {
                foreach (var dynamicType in dynamicTypes)
                {
                    var dynamicModule = ModuleBuilderManager.GetModules().FirstOrDefault(m => m.Id == dynamicType.ParentModuleId);
                    var areaName = this.GetDynamicTypeAreaName(dynamicModule.Title, dynamicType.DisplayName);
                    if (areaName == null) continue;

                    areaNames.Add(areaName);
                }
            }
            else
            {
                areaNames.Add(controllerName);
            }

            return areaNames.ToArray();
        }

        /// <summary>
        /// Gets the area name for dynamic content MVC widget.
        /// </summary>
        /// <param name="dynamicModuleName">Name of the dynamic module.</param>
        /// <param name="dynamicModuleType">Type of the dynamic module.</param>
        /// <returns>Area name.</returns>
        private string GetDynamicTypeAreaName(string dynamicModuleName, string dynamicModuleType)
        {
            return string.Format(CultureInfo.InvariantCulture, DatabaseResourceResolver.DynamicTypeAreaNameTemplate, dynamicModuleName, dynamicModuleType);
        }

        /// <summary>
        /// Gets the full names of <see cref="Controller"/>, located in the assembly that is specified in the <see cref="PathDefinition"/>.
        /// </summary>
        /// <param name="definition">The path definition.</param>
        /// <returns>The full names of <see cref="Controller"/>.</returns>
        private IEnumerable<string> GetControllersFullNames(PathDefinition definition)
        {
            var assembly = this.GetAssembly(definition);
            if (assembly == null)
                return null;

            return assembly.GetExportedTypes().Where(FrontendManager.ControllerFactory.IsController).Select(c => c.FullName);
        }

        private string GetExistsCacheKey(PathDefinition definition, string virtualPath)
        {
            string moduleName = definition.Parameters[ModuleNameParam];
            return "{0}_{1}_{2}_Exists_{3}".Arrange(this.GetType().Name, moduleName, definition.ResolverName, virtualPath.GetHashCode());
        }

        private string GetFilesCacheKey(PathDefinition definition, string virtualPath)
        {
            string moduleName = definition.Parameters[ModuleNameParam];
            return "{0}_{1}_{2}_GetFiles_{3}".Arrange(this.GetType().Name, moduleName, definition.ResolverName, virtualPath.GetHashCode());
        }

        /// <summary>Template for area name used by dynamic content MVC widget</summary>
        internal static readonly string DynamicTypeAreaNameTemplate = "{0} - {1}";

        /// <summary>Template for view path, consisting of path and file name</summary>
        internal static readonly string ViewPathTemplate = "{0}{1}.cshtml";

        /// <summary>
        /// Lock per instance since the cache key depends on the current resolver instance. Multiple instances won't clash.
        /// </summary>
        private readonly object existsLock = new object();

        /// <summary>
        /// Lock per instance since the cache key depends on the current resolver instance. Multiple instances won't clash.
        /// </summary>
        private readonly object getFilesLock = new object();

        /// <summary>
        /// Relative path for field templates
        /// </summary>
        private const string ControlPresentationViewPathPattern = @"Views[/|\\][^/\\]+[/|\\][^/\\]+\.cshtml";

        /// <summary>
        /// Name of module name parameters
        /// </summary>
        private const string ModuleNameParam = "ModuleName";
    }
}
