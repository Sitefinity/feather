using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Pages.Model;
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
            var controlPresentation = this.GetControlPresentation(definition, virtualPath);
            return controlPresentation != null && !controlPresentation.Data.IsNullOrEmpty();
        }

        /// <inheritdoc />
        protected override CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var controlPresentation = this.GetControlPresentation(definition, virtualPath);
            if (controlPresentation != null)
            {
                return new ControlPresentationCacheDependency(controlPresentation.Id.ToString());
            }

            // Change to any ControlPresentation record will invalidate the cache for this virtual path.
            return new ControlPresentationCacheDependency(typeof(ControlPresentation));
        }

        /// <inheritdoc />
        protected override Stream CurrentOpen(PathDefinition definition, string virtualPath)
        {
            var controlPresentation = this.GetControlPresentation(definition, virtualPath);
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

            var controllerName = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            
            if (controllerName == null)
                return null;
            
            var controllers = this.GetControllersFullNames(definition);
            
            if (controllers == null)
                return null;

            var dynamicType = ControllerExtensions.GetDynamicContentType(controllerName);
            var areaName = definition.ResolverName;

            // case for dynamic type
            if (dynamicType != null)
                areaName = this.GetDynamicTypeAreaName(dynamicType.GetModuleName(), dynamicType.DisplayName);

            return this.GetViewPaths(path, controllers, areaName);
        }

        /// <summary>
        /// Gets the control presentation that contains the requested resource from the database.
        /// </summary>
        /// <param name="virtualPathDefinition">The virtual path definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        protected virtual ControlPresentation GetControlPresentation(PathDefinition virtualPathDefinition, string virtualPath)
        {
            if (virtualPathDefinition == null)
                throw new ArgumentNullException("virtualPathDefinition");

            if (virtualPath == null)
                throw new ArgumentNullException("virtualPath");

            var extension = Path.GetExtension(virtualPath);

            /// TODO: Fix - currently allowed only for razor views
            if (extension == MvcConstants.RazorFileNameExtension)
            {
                var name = Path.GetFileNameWithoutExtension(virtualPath);

                var controllers = this.GetControllersFullNames(virtualPathDefinition);

                if (controllers == null)
                    return null;

                var pathNames = virtualPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (pathNames == null)
                    return null;

                var controllerName = string.Empty;

                if (pathNames.Length > 0)
                    controllerName = pathNames[pathNames.Length - 2];

                var dynamicType = ControllerExtensions.GetDynamicContentType(controllerName);

                var areaName = virtualPathDefinition.ResolverName;

                // case for dynamic types
                if (dynamicType != null)
                    areaName = this.GetDynamicTypeAreaName(dynamicType.GetModuleName(), dynamicType.DisplayName);

                return this.GetControlPresentationItem(controllers, name, areaName);
            }

            return null;
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
            var views = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(t => controllers.Contains(t.ControlType) && t.AreaName == areaName)
                                            .ToArray();

            if (views == null)
                return null;

            var viewPaths = views.Select(t => string.Format(CultureInfo.InvariantCulture, DatabaseResourceResolver.ViewPathTemplate, path, t.Name));

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
        private ControlPresentation GetControlPresentationItem(IEnumerable<string> controllers, string name, string areaName)
        {
            var returnResult = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(t => controllers.Contains(t.ControlType))
                                            .FirstOrDefault(cp => cp.Name == name && cp.AreaName == areaName);

            return returnResult;
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

        /// <summary>Template for area name used by dynamic content MVC widget</summary>
        internal static readonly string DynamicTypeAreaNameTemplate = "{0} - {1}";

        /// <summary>Template for view path, consisting of path and file name</summary>
        internal static readonly string ViewPathTemplate = "{0}{1}.cshtml";
    }
}
