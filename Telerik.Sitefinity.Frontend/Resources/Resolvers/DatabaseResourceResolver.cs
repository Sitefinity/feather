using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Modules.Pages;
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

            var controllers = this.GetRelevantControllers(definition);

            if (controllers == null)
                return null;

            var dynamicType = ControllerExtensions.GetDynamicContentType(controllerName);

            // case for dynamic type
            if (dynamicType != null)
            {
                var dynamicAreaName = this.GetDynamicAreaName(dynamicType.GetModuleName(), dynamicType.DisplayName);
                return this.GetTemplatePaths(path, controllers, dynamicAreaName);
            }

            var templatePaths = this.GetTemplatePaths(path, controllers, definition.ResolverName);

            return templatePaths;
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
            if (extension == ".cshtml")
            {
                var name = Path.GetFileNameWithoutExtension(virtualPath);

                var controllers = this.GetRelevantControllers(virtualPathDefinition);

                if (controllers == null)
                    return null;

                var pathNames = virtualPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (pathNames == null)
                    return null;

                var controllerName = string.Empty;

                if (pathNames.Length > 0)
                    controllerName = pathNames[pathNames.Length - 2];

                var dynamicType = ControllerExtensions.GetDynamicContentType(controllerName);

                // case for dynamic types
                if (dynamicType != null)
                {
                    var dynamicAreaName = this.GetDynamicAreaName(dynamicType.GetModuleName(), dynamicType.DisplayName);
                    return this.GetControlPresentationItem(controllers, name, dynamicAreaName);
                }

                return this.GetControlPresentationItem(controllers, name, virtualPathDefinition.ResolverName);
            }

            return null;
        }

        /// <summary>
        /// Gets the template paths available for current widget.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="controllers">The controllers.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>currently available controll paths</returns>
        private IEnumerable<string> GetTemplatePaths(string path, IEnumerable<string> controllers, string areaName)
        {
            var dynamicTemplates = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(t => controllers.Contains(t.ControlType) && t.AreaName == areaName)
                                            .ToArray();

            if (dynamicTemplates == null)
                return null;

            var dynamicTemplatesPaths = dynamicTemplates.Select(t => string.Format(CultureInfo.InvariantCulture, "{0}{1}.cshtml", path, t.Name));

            return dynamicTemplatesPaths;
        }

        /// <summary>
        /// Gets the widget
        /// </summary>
        /// <param name="controllers">The controllers.</param>
        /// <param name="name">The name.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>ControlPresentationItem</returns>
        private ControlPresentation GetControlPresentationItem(IEnumerable<string> controllers, string name, string areaName)
        {
            var returnResult = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(t => controllers.Contains(t.ControlType))
                                            .FirstOrDefault(cp => cp.Name == name && cp.AreaName == areaName);

            return returnResult;
        }

        /// <summary>
        /// Gets the name of the dynamic area.
        /// </summary>
        /// <param name="dynamicModuleName">Name of the dynamic module.</param>
        /// <param name="dynamicModuleType">Type of the dynamic module.</param>
        /// <returns>Area name</returns>
        private string GetDynamicAreaName(string dynamicModuleName, string dynamicModuleType)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} - {1}", dynamicModuleName, dynamicModuleType);
        }

        private IEnumerable<string> GetRelevantControllers(PathDefinition definition)
        {
            var assembly = this.GetAssembly(definition);
            if (assembly == null)
                return null;

            return assembly.GetExportedTypes().Where(FrontendManager.ControllerFactory.IsController).Select(c => c.FullName);
        }
    }
}
