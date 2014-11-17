using System;
using System.Collections;
using System.Collections.Generic;
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
                var dynamicAreaName = string.Format("{0} - {1}", dynamicType.GetModuleName(), dynamicType.DisplayName);
                var dynamicTemplates = PageManager.GetManager().GetPresentationItems<ControlPresentation>().Where(t => controllers.Contains(t.ControlType) && t.AreaName == dynamicAreaName).ToList();

                var dynamicTemplatesPath = dynamicTemplates.Select(t => string.Format("{0}{1}.cshtml", path, t.Name));

                return dynamicTemplatesPath;
            }

            var templates = PageManager.GetManager().GetPresentationItems<ControlPresentation>().Where(t => controllers.Contains(t.ControlType) && t.AreaName == definition.ResolverName);

            var templatePaths = templates.Select(t => string.Format("{0}{1}.cshtml", path, t.Name));

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
            ControlPresentation controlPresentation = null;

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
                    var dynamicAreaName = string.Format("{0} - {1}", dynamicType.GetModuleName(), dynamicType.DisplayName);

                    controlPresentation = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(cp => controllers.Contains(cp.ControlType))
                                            .FirstOrDefault(cp => cp.Name == name && cp.AreaName == dynamicAreaName);

                    return controlPresentation;
                }

                var areaName = virtualPathDefinition.ResolverName;

                controlPresentation = PageManager.GetManager().GetPresentationItems<ControlPresentation>()
                                            .Where(t => controllers.Contains(t.ControlType))
                                            .FirstOrDefault(cp => cp.Name == name && cp.AreaName == areaName);
            }

            return controlPresentation;
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
